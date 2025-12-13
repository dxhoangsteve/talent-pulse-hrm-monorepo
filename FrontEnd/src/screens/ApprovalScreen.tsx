import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, ScrollView, TouchableOpacity, Alert, ActivityIndicator, TextInput, Modal } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import { ArrowLeft, Check, X, Clock, Calendar, RefreshCw } from 'lucide-react-native';
import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList, LeaveRequestItem, OTRequestItem, RequestStatus, getStatusLabel, getLeaveTypeLabel } from '../types/types';
import { leaveRequestService } from '../services/leaveRequestService';
import { overtimeRequestService } from '../services/overtimeRequestService';

type Props = NativeStackScreenProps<RootStackParamList, 'ApprovalScreen'>;

type TabType = 'leave' | 'ot';

export default function ApprovalScreen({ navigation }: Props) {
  const [activeTab, setActiveTab] = useState<TabType>('leave');
  const [leaveRequests, setLeaveRequests] = useState<LeaveRequestItem[]>([]);
  const [otRequests, setOtRequests] = useState<OTRequestItem[]>([]);
  const [loading, setLoading] = useState(true);
  
  // Reject modal
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [rejectReason, setRejectReason] = useState('');
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [selectedType, setSelectedType] = useState<TabType>('leave');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setLoading(true);
    const [leaveResult, otResult] = await Promise.all([
      leaveRequestService.getPendingForApproval(),
      overtimeRequestService.getPendingForApproval(),
    ]);

    if (leaveResult.isSuccessed && leaveResult.resultObj) {
      setLeaveRequests(leaveResult.resultObj);
    }
    if (otResult.isSuccessed && otResult.resultObj) {
      setOtRequests(otResult.resultObj);
    }
    setLoading(false);
  };

  const handleApprove = async (id: string, type: TabType) => {
    Alert.alert('Xác nhận duyệt', 'Bạn có chắc muốn duyệt đơn này?', [
      { text: 'Hủy', style: 'cancel' },
      {
        text: 'Duyệt',
        onPress: async () => {
          const result = type === 'leave' 
            ? await leaveRequestService.approveRequest(id)
            : await overtimeRequestService.approveRequest(id);
          
          if (result.isSuccessed) {
            Alert.alert('Thành công', 'Đã duyệt đơn');
            loadData();
          } else {
            Alert.alert('Lỗi', result.message || 'Không thể duyệt đơn');
          }
        },
      },
    ]);
  };

  const openRejectModal = (id: string, type: TabType) => {
    setSelectedId(id);
    setSelectedType(type);
    setRejectReason('');
    setShowRejectModal(true);
  };

  const handleReject = async () => {
    if (!selectedId) return;

    const result = selectedType === 'leave'
      ? await leaveRequestService.rejectRequest(selectedId, rejectReason)
      : await overtimeRequestService.rejectRequest(selectedId, rejectReason);

    if (result.isSuccessed) {
      Alert.alert('Thành công', 'Đã từ chối đơn');
      setShowRejectModal(false);
      loadData();
    } else {
      Alert.alert('Lỗi', result.message || 'Không thể từ chối đơn');
    }
  };

  const pendingLeaveCount = leaveRequests.filter(r => r.status === RequestStatus.Pending).length;
  const pendingOTCount = otRequests.filter(r => r.status === RequestStatus.Pending).length;

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backBtn}>
          <ArrowLeft size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Duyệt đơn</Text>
        <TouchableOpacity onPress={loadData} style={styles.refreshBtn}>
          <RefreshCw size={20} color={Colors.primary} />
        </TouchableOpacity>
      </View>

      {/* Tabs */}
      <View style={styles.tabContainer}>
        <TouchableOpacity
          style={[styles.tab, activeTab === 'leave' && styles.tabActive]}
          onPress={() => setActiveTab('leave')}
        >
          <Calendar size={18} color={activeTab === 'leave' ? Colors.primary : Colors.textSecondary} />
          <Text style={[styles.tabText, activeTab === 'leave' && styles.tabTextActive]}>
            Nghỉ phép
          </Text>
          {pendingLeaveCount > 0 && (
            <View style={styles.badge}>
              <Text style={styles.badgeText}>{pendingLeaveCount}</Text>
            </View>
          )}
        </TouchableOpacity>

        <TouchableOpacity
          style={[styles.tab, activeTab === 'ot' && styles.tabActive]}
          onPress={() => setActiveTab('ot')}
        >
          <Clock size={18} color={activeTab === 'ot' ? Colors.primary : Colors.textSecondary} />
          <Text style={[styles.tabText, activeTab === 'ot' && styles.tabTextActive]}>
            OT
          </Text>
          {pendingOTCount > 0 && (
            <View style={styles.badge}>
              <Text style={styles.badgeText}>{pendingOTCount}</Text>
            </View>
          )}
        </TouchableOpacity>
      </View>

      {/* Content */}
      <ScrollView contentContainerStyle={styles.content}>
        {loading ? (
          <ActivityIndicator size="large" color={Colors.primary} style={{ marginTop: 40 }} />
        ) : activeTab === 'leave' ? (
          leaveRequests.length === 0 ? (
            <View style={styles.emptyState}>
              <Calendar size={48} color={Colors.textLight} />
              <Text style={styles.emptyText}>Không có đơn nghỉ phép chờ duyệt</Text>
            </View>
          ) : (
            leaveRequests.map((item) => (
              <View key={item.id} style={styles.requestCard}>
                <View style={styles.cardHeader}>
                  <Text style={styles.employeeName}>{item.employeeName}</Text>
                  <Text style={styles.leaveType}>{getLeaveTypeLabel(item.leaveType)}</Text>
                </View>

                <View style={styles.cardBody}>
                  <Text style={styles.dateText}>
                    {new Date(item.startDate).toLocaleDateString('vi-VN')} - {new Date(item.endDate).toLocaleDateString('vi-VN')}
                  </Text>
                  <Text style={styles.totalDays}>{item.totalDays} ngày</Text>
                </View>

                <View style={styles.actionButtons}>
                  <TouchableOpacity
                    style={[styles.actionBtn, styles.rejectBtn]}
                    onPress={() => openRejectModal(item.id, 'leave')}
                  >
                    <X size={18} color="white" />
                    <Text style={styles.actionBtnText}>Từ chối</Text>
                  </TouchableOpacity>
                  <TouchableOpacity
                    style={[styles.actionBtn, styles.approveBtn]}
                    onPress={() => handleApprove(item.id, 'leave')}
                  >
                    <Check size={18} color="white" />
                    <Text style={styles.actionBtnText}>Duyệt</Text>
                  </TouchableOpacity>
                </View>
              </View>
            ))
          )
        ) : (
          otRequests.length === 0 ? (
            <View style={styles.emptyState}>
              <Clock size={48} color={Colors.textLight} />
              <Text style={styles.emptyText}>Không có đơn OT chờ duyệt</Text>
            </View>
          ) : (
            otRequests.map((item) => (
              <View key={item.id} style={styles.requestCard}>
                <View style={styles.cardHeader}>
                  <Text style={styles.employeeName}>{item.employeeName}</Text>
                  <Text style={styles.multiplier}>x{item.multiplier}</Text>
                </View>

                <View style={styles.cardBody}>
                  <Text style={styles.dateText}>
                    {new Date(item.otDate).toLocaleDateString('vi-VN')} | {item.startTime.substring(0, 5)} - {item.endTime.substring(0, 5)}
                  </Text>
                  <Text style={styles.totalDays}>{item.hours.toFixed(1)}h</Text>
                </View>

                <View style={styles.actionButtons}>
                  <TouchableOpacity
                    style={[styles.actionBtn, styles.rejectBtn]}
                    onPress={() => openRejectModal(item.id, 'ot')}
                  >
                    <X size={18} color="white" />
                    <Text style={styles.actionBtnText}>Từ chối</Text>
                  </TouchableOpacity>
                  <TouchableOpacity
                    style={[styles.actionBtn, styles.approveBtn]}
                    onPress={() => handleApprove(item.id, 'ot')}
                  >
                    <Check size={18} color="white" />
                    <Text style={styles.actionBtnText}>Duyệt</Text>
                  </TouchableOpacity>
                </View>
              </View>
            ))
          )
        )}
      </ScrollView>

      {/* Reject Modal */}
      <Modal visible={showRejectModal} transparent animationType="fade">
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <Text style={styles.modalTitle}>Lý do từ chối</Text>
            <TextInput
              style={styles.modalInput}
              placeholder="Nhập lý do từ chối (không bắt buộc)"
              value={rejectReason}
              onChangeText={setRejectReason}
              multiline
              numberOfLines={3}
            />
            <View style={styles.modalButtons}>
              <TouchableOpacity style={styles.modalCancelBtn} onPress={() => setShowRejectModal(false)}>
                <Text style={styles.modalCancelText}>Hủy</Text>
              </TouchableOpacity>
              <TouchableOpacity style={styles.modalConfirmBtn} onPress={handleReject}>
                <Text style={styles.modalConfirmText}>Xác nhận</Text>
              </TouchableOpacity>
            </View>
          </View>
        </View>
      </Modal>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: Colors.background },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: Spacing.lg,
    backgroundColor: 'white',
    borderBottomWidth: 1,
    borderBottomColor: Colors.border,
  },
  backBtn: { padding: Spacing.xs },
  headerTitle: { fontSize: FontSize.lg, fontWeight: 'bold', color: Colors.text },
  refreshBtn: { padding: Spacing.xs },
  
  // Tabs
  tabContainer: {
    flexDirection: 'row',
    backgroundColor: 'white',
    paddingHorizontal: Spacing.lg,
    borderBottomWidth: 1,
    borderBottomColor: Colors.border,
  },
  tab: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: Spacing.md,
    gap: Spacing.xs,
  },
  tabActive: { borderBottomWidth: 2, borderBottomColor: Colors.primary },
  tabText: { fontSize: FontSize.sm, color: Colors.textSecondary },
  tabTextActive: { color: Colors.primary, fontWeight: '600' },
  badge: {
    backgroundColor: Colors.error,
    borderRadius: 10,
    minWidth: 20,
    height: 20,
    alignItems: 'center',
    justifyContent: 'center',
    marginLeft: 4,
  },
  badgeText: { color: 'white', fontSize: FontSize.xs, fontWeight: 'bold' },
  
  content: { padding: Spacing.lg },
  
  // Empty
  emptyState: { alignItems: 'center', marginTop: 60 },
  emptyText: { color: Colors.textSecondary, fontSize: FontSize.md, marginTop: Spacing.md },
  
  // Card
  requestCard: {
    backgroundColor: 'white',
    borderRadius: BorderRadius.md,
    padding: Spacing.md,
    marginBottom: Spacing.md,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  cardHeader: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: Spacing.sm },
  employeeName: { fontSize: FontSize.md, fontWeight: 'bold', color: Colors.text },
  leaveType: { fontSize: FontSize.sm, color: Colors.primary, fontWeight: '600' },
  multiplier: { fontSize: FontSize.sm, color: Colors.secondary, fontWeight: 'bold' },
  cardBody: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: Spacing.md },
  dateText: { color: Colors.textSecondary, fontSize: FontSize.sm },
  totalDays: { fontSize: FontSize.lg, fontWeight: 'bold', color: Colors.primary },
  
  // Actions
  actionButtons: { flexDirection: 'row', gap: Spacing.sm },
  actionBtn: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: Spacing.sm,
    borderRadius: BorderRadius.sm,
    gap: Spacing.xs,
  },
  rejectBtn: { backgroundColor: Colors.error },
  approveBtn: { backgroundColor: Colors.success },
  actionBtnText: { color: 'white', fontWeight: '600', fontSize: FontSize.sm },
  
  // Modal
  modalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0,0,0,0.5)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  modalContent: {
    backgroundColor: 'white',
    borderRadius: BorderRadius.md,
    padding: Spacing.lg,
    width: '85%',
  },
  modalTitle: { fontSize: FontSize.lg, fontWeight: 'bold', color: Colors.text, marginBottom: Spacing.md },
  modalInput: {
    backgroundColor: Colors.background,
    borderRadius: BorderRadius.sm,
    padding: Spacing.md,
    fontSize: FontSize.md,
    height: 80,
    textAlignVertical: 'top',
    marginBottom: Spacing.md,
  },
  modalButtons: { flexDirection: 'row', gap: Spacing.sm },
  modalCancelBtn: {
    flex: 1,
    padding: Spacing.md,
    borderRadius: BorderRadius.sm,
    backgroundColor: Colors.background,
    alignItems: 'center',
  },
  modalCancelText: { color: Colors.textSecondary, fontWeight: '600' },
  modalConfirmBtn: {
    flex: 1,
    padding: Spacing.md,
    borderRadius: BorderRadius.sm,
    backgroundColor: Colors.error,
    alignItems: 'center',
  },
  modalConfirmText: { color: 'white', fontWeight: '600' },
});
