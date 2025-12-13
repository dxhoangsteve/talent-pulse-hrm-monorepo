import React, { useState, useEffect, useCallback } from 'react';
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  TouchableOpacity,
  ActivityIndicator,
  RefreshControl,
  Alert,
  Modal,
  ScrollView,
} from 'react-native';
import { overtimeRequestService } from '../services/overtimeRequestService';
import { departmentService, Department } from '../services/departmentService';
import { OTRequestItem } from '../types/types';
import { useNavigation } from '@react-navigation/native';

const statusOptions = [
  { label: 'Tất cả', value: -1 },
  { label: 'Chờ duyệt', value: 0 },
  { label: 'Đã duyệt', value: 1 },
  { label: 'Từ chối', value: 2 },
  { label: 'Đã hủy', value: 3 },
];

const OTHistoryScreen: React.FC = () => {
  const navigation = useNavigation();
  const [requests, setRequests] = useState<OTRequestItem[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  
  const [selectedDept, setSelectedDept] = useState<string>('');
  const [selectedStatus, setSelectedStatus] = useState<number>(-1);
  const [showDeptModal, setShowDeptModal] = useState(false);
  const [showStatusModal, setShowStatusModal] = useState(false);
  
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const pageSize = 15;

  const loadDepartments = useCallback(async () => {
    const result = await departmentService.getDepartments();
    if (result) setDepartments(result);
  }, []);

  const loadData = useCallback(async (page = 1) => {
    try {
      setLoading(true);
      const result = await overtimeRequestService.getAllRequests({
        departmentId: selectedDept || undefined,
        status: selectedStatus >= 0 ? selectedStatus : undefined,
        page,
        pageSize,
      });
      if (result.isSuccessed && result.resultObj) {
        setRequests(result.resultObj.items);
        setTotalPages(result.resultObj.totalPages);
        setCurrentPage(result.resultObj.pageIndex);
      }
    } catch (error) {
      Alert.alert('Lỗi', 'Không thể tải dữ liệu');
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, [selectedDept, selectedStatus]);

  useEffect(() => { loadDepartments(); }, [loadDepartments]);
  useEffect(() => { loadData(1); }, [selectedDept, selectedStatus, loadData]);

  const handleRefresh = () => { setRefreshing(true); loadData(1); };

  const getDeptName = () => {
    if (!selectedDept) return 'Tất cả';
    return departments.find(d => d.id === selectedDept)?.name || 'Tất cả';
  };

  const getStatusLabel = () => statusOptions.find(o => o.value === selectedStatus)?.label || 'Tất cả';

  const getStatusStyle = (status: string | number) => {
    const s = String(status);
    switch (s) {
      case 'Pending': case '0': return styles.statusPending;
      case 'Approved': case '1': return styles.statusApproved;
      case 'Rejected': case '2': return styles.statusRejected;
      case 'Cancelled': case '3': return styles.statusCancelled;
      default: return {};
    }
  };

  const getStatusText = (status: string | number) => {
    const s = String(status);
    switch (s) {
      case 'Pending': case '0': return 'Chờ duyệt';
      case 'Approved': case '1': return 'Đã duyệt';
      case 'Rejected': case '2': return 'Từ chối';
      case 'Cancelled': case '3': return 'Đã hủy';
      default: return s;
    }
  };

  const formatDate = (dateStr: string) => {
    const date = new Date(dateStr);
    return `${date.getDate()}/${date.getMonth() + 1}/${date.getFullYear()}`;
  };

  const formatTime = (time: string) => {
    if (!time) return '';
    const parts = time.split(':');
    return `${parts[0]}:${parts[1]}`;
  };

  const renderItem = ({ item }: { item: OTRequestItem }) => (
    <View style={styles.card}>
      <View style={styles.cardHeader}>
        <Text style={styles.employeeName}>{item.employeeName}</Text>
        <View style={[styles.statusBadge, getStatusStyle(item.status)]}>
          <Text style={styles.statusText}>{getStatusText(item.status)}</Text>
        </View>
      </View>
      <Text style={styles.dateText}>{formatDate(item.otDate)}</Text>
      <Text style={styles.timeText}>
        {formatTime(item.startTime)} - {formatTime(item.endTime)} ({item.hours}h × {item.multiplier})
      </Text>
      {item.approvedByName && (
        <Text style={styles.approver}>Duyệt bởi: {item.approvedByName}</Text>
      )}
    </View>
  );

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backBtn}>
          <Text style={styles.backText}>← Quay lại</Text>
        </TouchableOpacity>
        <Text style={styles.title}>Lịch sử Tăng ca</Text>
      </View>

      <View style={styles.filterRow}>
        <View style={styles.filterItem}>
          <Text style={styles.filterLabel}>Phòng ban:</Text>
          <TouchableOpacity style={styles.dropdown} onPress={() => setShowDeptModal(true)}>
            <Text style={styles.dropdownText}>{getDeptName()}</Text>
            <Text style={styles.dropdownArrow}>▼</Text>
          </TouchableOpacity>
        </View>
        <View style={styles.filterItem}>
          <Text style={styles.filterLabel}>Trạng thái:</Text>
          <TouchableOpacity style={styles.dropdown} onPress={() => setShowStatusModal(true)}>
            <Text style={styles.dropdownText}>{getStatusLabel()}</Text>
            <Text style={styles.dropdownArrow}>▼</Text>
          </TouchableOpacity>
        </View>
      </View>

      {loading && !refreshing ? (
        <ActivityIndicator size="large" color="#FF9800" style={styles.loader} />
      ) : (
        <FlatList
          data={requests}
          keyExtractor={item => item.id}
          renderItem={renderItem}
          contentContainerStyle={styles.listContent}
          refreshControl={<RefreshControl refreshing={refreshing} onRefresh={handleRefresh} />}
          ListEmptyComponent={<Text style={styles.emptyText}>Không có dữ liệu</Text>}
        />
      )}

      {totalPages > 1 && (
        <View style={styles.pagination}>
          <TouchableOpacity
            style={[styles.pageBtn, currentPage <= 1 && styles.pageBtnDisabled]}
            onPress={() => loadData(currentPage - 1)}
            disabled={currentPage <= 1}>
            <Text style={styles.pageBtnText}>« Trước</Text>
          </TouchableOpacity>
          <Text style={styles.pageInfo}>Trang {currentPage}/{totalPages}</Text>
          <TouchableOpacity
            style={[styles.pageBtn, currentPage >= totalPages && styles.pageBtnDisabled]}
            onPress={() => loadData(currentPage + 1)}
            disabled={currentPage >= totalPages}>
            <Text style={styles.pageBtnText}>Sau »</Text>
          </TouchableOpacity>
        </View>
      )}

      {/* Department Modal */}
      <Modal visible={showDeptModal} transparent animationType="slide">
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <Text style={styles.modalTitle}>Chọn phòng ban</Text>
            <ScrollView style={styles.modalScroll}>
              <TouchableOpacity style={styles.modalOption} onPress={() => { setSelectedDept(''); setShowDeptModal(false); }}>
                <Text style={[styles.modalOptionText, !selectedDept && styles.modalOptionSelected]}>Tất cả</Text>
              </TouchableOpacity>
              {departments.map(d => (
                <TouchableOpacity key={d.id} style={styles.modalOption} onPress={() => { setSelectedDept(d.id); setShowDeptModal(false); }}>
                  <Text style={[styles.modalOptionText, selectedDept === d.id && styles.modalOptionSelected]}>{d.name}</Text>
                </TouchableOpacity>
              ))}
            </ScrollView>
            <TouchableOpacity style={styles.modalClose} onPress={() => setShowDeptModal(false)}>
              <Text style={styles.modalCloseText}>Đóng</Text>
            </TouchableOpacity>
          </View>
        </View>
      </Modal>

      {/* Status Modal */}
      <Modal visible={showStatusModal} transparent animationType="slide">
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <Text style={styles.modalTitle}>Chọn trạng thái</Text>
            <ScrollView style={styles.modalScroll}>
              {statusOptions.map(opt => (
                <TouchableOpacity key={opt.value} style={styles.modalOption} onPress={() => { setSelectedStatus(opt.value); setShowStatusModal(false); }}>
                  <Text style={[styles.modalOptionText, selectedStatus === opt.value && styles.modalOptionSelected]}>{opt.label}</Text>
                </TouchableOpacity>
              ))}
            </ScrollView>
            <TouchableOpacity style={styles.modalClose} onPress={() => setShowStatusModal(false)}>
              <Text style={styles.modalCloseText}>Đóng</Text>
            </TouchableOpacity>
          </View>
        </View>
      </Modal>
    </View>
  );
};

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#f5f5f5' },
  header: { flexDirection: 'row', alignItems: 'center', backgroundColor: '#FF9800', padding: 16, paddingTop: 48 },
  backBtn: { marginRight: 16 },
  backText: { color: '#fff', fontSize: 16 },
  title: { color: '#fff', fontSize: 20, fontWeight: 'bold' },
  filterRow: { flexDirection: 'row', padding: 12, backgroundColor: '#fff' },
  filterItem: { flex: 1, marginHorizontal: 4 },
  filterLabel: { fontSize: 12, color: '#666', marginBottom: 4 },
  dropdown: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', backgroundColor: '#f0f0f0', borderRadius: 8, padding: 12 },
  dropdownText: { fontSize: 14, color: '#333' },
  dropdownArrow: { fontSize: 10, color: '#666' },
  listContent: { padding: 12 },
  loader: { marginTop: 40 },
  card: { backgroundColor: '#fff', borderRadius: 12, padding: 16, marginBottom: 12, elevation: 2 },
  cardHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 },
  employeeName: { fontSize: 16, fontWeight: '600', color: '#333' },
  statusBadge: { paddingHorizontal: 10, paddingVertical: 4, borderRadius: 12 },
  statusText: { fontSize: 12, fontWeight: '600', color: '#fff' },
  statusPending: { backgroundColor: '#FFA000' },
  statusApproved: { backgroundColor: '#4CAF50' },
  statusRejected: { backgroundColor: '#F44336' },
  statusCancelled: { backgroundColor: '#9E9E9E' },
  dateText: { fontSize: 15, color: '#333', fontWeight: '500' },
  timeText: { fontSize: 14, color: '#555', marginTop: 2 },
  approver: { fontSize: 12, color: '#888', marginTop: 4, fontStyle: 'italic' },
  emptyText: { textAlign: 'center', color: '#999', marginTop: 40 },
  pagination: { flexDirection: 'row', justifyContent: 'center', alignItems: 'center', padding: 12, backgroundColor: '#fff', borderTopWidth: 1, borderTopColor: '#eee' },
  pageBtn: { backgroundColor: '#FF9800', paddingHorizontal: 16, paddingVertical: 8, borderRadius: 8, marginHorizontal: 8 },
  pageBtnDisabled: { backgroundColor: '#ccc' },
  pageBtnText: { color: '#fff', fontWeight: '600' },
  pageInfo: { color: '#666' },
  // Modal
  modalOverlay: { flex: 1, backgroundColor: 'rgba(0,0,0,0.5)', justifyContent: 'flex-end' },
  modalContent: { backgroundColor: '#fff', borderTopLeftRadius: 20, borderTopRightRadius: 20, maxHeight: '60%', padding: 16 },
  modalTitle: { fontSize: 18, fontWeight: 'bold', marginBottom: 12, textAlign: 'center' },
  modalScroll: { maxHeight: 300 },
  modalOption: { paddingVertical: 12, borderBottomWidth: 1, borderBottomColor: '#eee' },
  modalOptionText: { fontSize: 16, color: '#333' },
  modalOptionSelected: { color: '#FF9800', fontWeight: '600' },
  modalClose: { marginTop: 12, padding: 12, backgroundColor: '#f0f0f0', borderRadius: 8, alignItems: 'center' },
  modalCloseText: { fontSize: 16, color: '#666' },
});

export default OTHistoryScreen;
