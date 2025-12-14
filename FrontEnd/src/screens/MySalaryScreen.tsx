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
  TextInput,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { ArrowLeft, DollarSign, AlertCircle, X, FileText, Send, CheckCircle } from 'lucide-react-native';
import { useNavigation } from '@react-navigation/native';
import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import { salaryService, SalaryVm, CreateComplaintRequest, ComplaintVm } from '../services/salaryService';

const COMPLAINT_TYPES = [
  { value: 0, label: 'Chưa nhận lương' },
  { value: 1, label: 'Sai số tiền' },
  { value: 2, label: 'Khác' },
];

export default function MySalaryScreen() {
  const navigation = useNavigation();
  const [salaries, setSalaries] = useState<SalaryVm[]>([]);
  const [complaints, setComplaints] = useState<ComplaintVm[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [selectedSalary, setSelectedSalary] = useState<SalaryVm | null>(null);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [showComplaintModal, setShowComplaintModal] = useState(false);
  const [complaintType, setComplaintType] = useState(0);
  const [complaintContent, setComplaintContent] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const loadData = useCallback(async () => {
    try {
      const [salaryRes, complaintRes] = await Promise.all([
        salaryService.getMySalary(),
        salaryService.getMyComplaints(),
      ]);
      if (salaryRes.isSuccessed) {
        setSalaries(salaryRes.resultObj || []);
      }
      if (complaintRes.isSuccessed) {
        setComplaints(complaintRes.resultObj || []);
      }
    } catch (error) {
      Alert.alert('Lỗi', 'Không thể tải dữ liệu lương');
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, []);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const handleRefresh = () => {
    setRefreshing(true);
    loadData();
  };

  const formatMoney = (amount: number) => {
    return amount.toLocaleString('vi-VN') + 'đ';
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Draft': return Colors.textSecondary;
      case 'Pending': return Colors.warning;
      case 'Approved': return Colors.primary;
      case 'Paid': return Colors.success;
      default: return Colors.textSecondary;
    }
  };

  const getStatusLabel = (status: string) => {
    switch (status) {
      case 'Draft': return 'Nháp';
      case 'Pending': return 'Chờ duyệt';
      case 'Approved': return 'Đã duyệt';
      case 'Paid': return 'Đã trả';
      default: return status;
    }
  };

  const handleViewDetail = (salary: SalaryVm) => {
    setSelectedSalary(salary);
    setShowDetailModal(true);
  };

  const handleOpenComplaint = (salary: SalaryVm) => {
    setSelectedSalary(salary);
    setComplaintType(0);
    setComplaintContent('');
    setShowComplaintModal(true);
  };

  const handleSubmitComplaint = async () => {
    if (!selectedSalary || !complaintContent.trim()) {
      Alert.alert('Lỗi', 'Vui lòng nhập nội dung khiếu nại');
      return;
    }

    setSubmitting(true);
    try {
      const request: CreateComplaintRequest = {
        month: selectedSalary.month,
        year: selectedSalary.year,
        complaintType: complaintType,
        content: complaintContent.trim(),
        salarySlipId: selectedSalary.id,
      };
      const result = await salaryService.createComplaint(request);
      if (result.isSuccessed) {
        Alert.alert('Thành công', 'Đã gửi khiếu nại');
        setShowComplaintModal(false);
        // Also update status to Complaining if needed, or just reload
        loadData();
      } else {
        Alert.alert('Lỗi', result.message || 'Không thể gửi khiếu nại');
      }
    } catch (error) {
      Alert.alert('Lỗi', 'Đã xảy ra lỗi khi gửi khiếu nại');
    } finally {
      setSubmitting(false);
    }
  };

  const handleConfirmSalary = async (salary: SalaryVm, isConfirmed: boolean) => {
    Alert.alert(
      'Xác nhận',
      'Bạn xác nhận đã nhận đủ lương và không có thắc mắc gì?',
      [
        { text: 'Hủy', style: 'cancel' },
        {
          text: 'Đồng ý',
          onPress: async () => {
            try {
              const result = await salaryService.confirmSalary(salary.id, true);
              if (result.isSuccessed) {
                Alert.alert('Thành công', 'Đã xác nhận lương');
                loadData();
              } else {
                Alert.alert('Lỗi', result.message || 'Có lỗi xảy ra');
              }
            } catch (error) {
              Alert.alert('Lỗi', 'Không thể xác nhận lương');
            }
          }
        }
      ]
    );
  };

  const hasComplaintForSalary = (salaryId: string) => {
    return complaints.some(c => c.month === salaries.find(s => s.id === salaryId)?.month);
  };

  const renderItem = ({ item }: { item: SalaryVm }) => (
    <TouchableOpacity style={styles.card} onPress={() => handleViewDetail(item)}>
      <View style={styles.cardHeader}>
        <View style={styles.monthBadge}>
          <Text style={styles.monthText}>T{item.month}/{item.year}</Text>
        </View>
        <View style={[styles.statusBadge, { backgroundColor: getStatusColor(item.status) + '20' }]}>
          <Text style={[styles.statusText, { color: getStatusColor(item.status) }]}>
            {getStatusLabel(item.status)}
          </Text>
        </View>
      </View>

      <View style={styles.cardBody}>
        <View style={styles.salaryRow}>
          <Text style={styles.label}>Lương cơ bản:</Text>
          <Text style={styles.value}>{formatMoney(item.baseSalary)}</Text>
        </View>
        <View style={styles.salaryRow}>
          <Text style={styles.label}>Lương OT:</Text>
          <Text style={[styles.value, { color: Colors.success }]}>+{formatMoney(item.overtimePay)}</Text>
        </View>
        <View style={styles.salaryRow}>
          <Text style={styles.label}>Thưởng:</Text>
          <Text style={[styles.value, { color: Colors.success }]}>+{formatMoney(item.bonus)}</Text>
        </View>
        <View style={styles.salaryRow}>
          <Text style={styles.label}>Khấu trừ:</Text>
          <Text style={[styles.value, { color: Colors.error }]}>-{formatMoney(item.deductions + item.insurance + item.tax)}</Text>
        </View>
        <View style={[styles.salaryRow, styles.netRow]}>
          <Text style={styles.netLabel}>Thực lãnh:</Text>
          <Text style={styles.netValue}>{formatMoney(item.netSalary)}</Text>
        </View>
      </View>

      {item.status === 'Paid' && !hasComplaintForSalary(item.id) && (
        <View style={styles.actionRow}>
          <TouchableOpacity
            style={[styles.actionBtn, styles.confirmBtn]}
            onPress={() => handleConfirmSalary(item, true)}
          >
            <CheckCircle size={16} color="white" />
            <Text style={styles.actionBtnText}>Đồng ý</Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={[styles.actionBtn, styles.complainBtn]}
            onPress={() => handleOpenComplaint(item)}
          >
            <AlertCircle size={16} color="white" />
            <Text style={styles.actionBtnText}>Khiếu nại</Text>
          </TouchableOpacity>
        </View>
      )}
    </TouchableOpacity>
  );

  if (loading) {
    return (
      <SafeAreaView style={styles.container}>
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color={Colors.primary} />
        </View>
      </SafeAreaView>
    );
  }

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()}>
          <ArrowLeft size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Phiếu lương của tôi</Text>
        <View style={{ width: 24 }} />
      </View>

      {/* Summary Card */}
      <View style={styles.summaryCard}>
        <DollarSign size={32} color={Colors.primary} />
        <View style={styles.summaryInfo}>
          <Text style={styles.summaryLabel}>Tổng số phiếu lương</Text>
          <Text style={styles.summaryValue}>{salaries.length}</Text>
        </View>
      </View>

      {/* Complaints Section */}
      {complaints.length > 0 && (
        <View style={styles.complaintsSection}>
          <Text style={styles.sectionTitle}>Khiếu nại của tôi ({complaints.length})</Text>
          <ScrollView horizontal showsHorizontalScrollIndicator={false}>
            {complaints.map(c => (
              <View key={c.id} style={styles.complaintCard}>
                <Text style={styles.complaintMonth}>T{c.month}/{c.year}</Text>
                <Text style={styles.complaintType}>{c.complaintTypeName}</Text>
                <Text style={[styles.complaintStatus, { color: c.status === 2 ? Colors.success : Colors.warning }]}>
                  {c.statusName}
                </Text>
              </View>
            ))}
          </ScrollView>
        </View>
      )}

      {/* Salary List */}
      <FlatList
        data={salaries}
        keyExtractor={item => item.id}
        renderItem={renderItem}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={handleRefresh} />
        }
        ListEmptyComponent={
          <View style={styles.emptyContainer}>
            <FileText size={48} color={Colors.textSecondary} />
            <Text style={styles.emptyText}>Chưa có phiếu lương nào</Text>
          </View>
        }
      />

      {/* Detail Modal */}
      <Modal visible={showDetailModal} animationType="slide" transparent>
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <View style={styles.modalHeader}>
              <Text style={styles.modalTitle}>Chi tiết phiếu lương</Text>
              <TouchableOpacity onPress={() => setShowDetailModal(false)}>
                <X size={24} color={Colors.text} />
              </TouchableOpacity>
            </View>
            {selectedSalary && (
              <ScrollView style={styles.modalBody}>
                <Text style={styles.detailSection}>Thông tin chung</Text>
                <View style={styles.detailRow}>
                  <Text style={styles.detailLabel}>Kỳ lương:</Text>
                  <Text style={styles.detailValue}>Tháng {selectedSalary.month}/{selectedSalary.year}</Text>
                </View>
                <View style={styles.detailRow}>
                  <Text style={styles.detailLabel}>Trạng thái:</Text>
                  <Text style={[styles.detailValue, { color: getStatusColor(selectedSalary.status) }]}>
                    {getStatusLabel(selectedSalary.status)}
                  </Text>
                </View>

                <Text style={styles.detailSection}>Ngày công</Text>
                <View style={styles.detailRow}>
                  <Text style={styles.detailLabel}>Ngày công chuẩn:</Text>
                  <Text style={styles.detailValue}>{selectedSalary.workDays} ngày</Text>
                </View>
                <View style={styles.detailRow}>
                  <Text style={styles.detailLabel}>Ngày công thực tế:</Text>
                  <Text style={styles.detailValue}>{selectedSalary.actualWorkDays} ngày</Text>
                </View>
                <View style={styles.detailRow}>
                  <Text style={styles.detailLabel}>Đi muộn:</Text>
                  <Text style={[styles.detailValue, { color: Colors.error }]}>{selectedSalary.lateDays} ngày</Text>
                </View>

                <Text style={styles.detailSection}>Thu nhập</Text>
                <View style={styles.detailRow}>
                  <Text style={styles.detailLabel}>Lương cơ bản:</Text>
                  <Text style={styles.detailValue}>{formatMoney(selectedSalary.baseSalary)}</Text>
                </View>
                <View style={styles.detailRow}>
                  <Text style={styles.detailLabel}>Lương OT:</Text>
                  <Text style={[styles.detailValue, { color: Colors.success }]}>+{formatMoney(selectedSalary.overtimePay)}</Text>
                </View>
                <View style={styles.detailRow}>
                  <Text style={styles.detailLabel}>Thưởng:</Text>
                  <Text style={[styles.detailValue, { color: Colors.success }]}>+{formatMoney(selectedSalary.bonus)}</Text>
                </View>
                <View style={styles.detailRow}>
                  <Text style={styles.detailLabel}>Phụ cấp:</Text>
                  <Text style={[styles.detailValue, { color: Colors.success }]}>+{formatMoney(selectedSalary.allowance)}</Text>
                </View>

                <Text style={styles.detailSection}>Khấu trừ</Text>
                <View style={styles.detailRow}>
                  <Text style={styles.detailLabel}>Bảo hiểm:</Text>
                  <Text style={[styles.detailValue, { color: Colors.error }]}>-{formatMoney(selectedSalary.insurance)}</Text>
                </View>
                <View style={styles.detailRow}>
                  <Text style={styles.detailLabel}>Thuế TNCN:</Text>
                  <Text style={[styles.detailValue, { color: Colors.error }]}>-{formatMoney(selectedSalary.tax)}</Text>
                </View>
                <View style={styles.detailRow}>
                  <Text style={styles.detailLabel}>Khấu trừ khác:</Text>
                  <Text style={[styles.detailValue, { color: Colors.error }]}>-{formatMoney(selectedSalary.deductions)}</Text>
                </View>

                <View style={styles.netSection}>
                  <Text style={styles.netLabel}>THỰC LÃNH</Text>
                  <Text style={styles.bigNetValue}>{formatMoney(selectedSalary.netSalary)}</Text>
                </View>

                {selectedSalary.paidTime && (
                  <Text style={styles.paidInfo}>
                    Đã thanh toán ngày: {new Date(selectedSalary.paidTime).toLocaleDateString('vi-VN')}
                  </Text>
                )}
              </ScrollView>
            )}
          </View>
        </View>
      </Modal>

      {/* Complaint Modal */}
      <Modal visible={showComplaintModal} animationType="slide" transparent>
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <View style={styles.modalHeader}>
              <Text style={styles.modalTitle}>Gửi khiếu nại</Text>
              <TouchableOpacity onPress={() => setShowComplaintModal(false)}>
                <X size={24} color={Colors.text} />
              </TouchableOpacity>
            </View>
            <View style={styles.modalBody}>
              {selectedSalary && (
                <Text style={styles.complaintPeriod}>
                  Kỳ lương: Tháng {selectedSalary.month}/{selectedSalary.year}
                </Text>
              )}

              <Text style={styles.inputLabel}>Loại khiếu nại</Text>
              <View style={styles.typeButtons}>
                {COMPLAINT_TYPES.map(type => (
                  <TouchableOpacity
                    key={type.value}
                    style={[styles.typeBtn, complaintType === type.value && styles.typeBtnActive]}
                    onPress={() => setComplaintType(type.value)}
                  >
                    <Text style={[styles.typeBtnText, complaintType === type.value && styles.typeBtnTextActive]}>
                      {type.label}
                    </Text>
                  </TouchableOpacity>
                ))}
              </View>

              <Text style={styles.inputLabel}>Nội dung khiếu nại</Text>
              <TextInput
                style={styles.textArea}
                multiline
                numberOfLines={4}
                placeholder="Mô tả chi tiết khiếu nại của bạn..."
                value={complaintContent}
                onChangeText={setComplaintContent}
              />

              <TouchableOpacity
                style={[styles.submitBtn, submitting && styles.submitBtnDisabled]}
                onPress={handleSubmitComplaint}
                disabled={submitting}
              >
                {submitting ? (
                  <ActivityIndicator color="white" />
                ) : (
                  <>
                    <Send size={20} color="white" />
                    <Text style={styles.submitBtnText}>Gửi khiếu nại</Text>
                  </>
                )}
              </TouchableOpacity>
            </View>
          </View>
        </View>
      </Modal>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  loadingContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: Spacing.lg,
    backgroundColor: 'white',
    borderBottomWidth: 1,
    borderBottomColor: Colors.border,
  },
  headerTitle: {
    fontSize: FontSize.lg,
    fontWeight: 'bold',
    color: Colors.text,
  },
  summaryCard: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.primary + '15',
    margin: Spacing.md,
    padding: Spacing.lg,
    borderRadius: BorderRadius.md,
    borderWidth: 1,
    borderColor: Colors.primary + '30',
  },
  summaryInfo: {
    marginLeft: Spacing.md,
  },
  summaryLabel: {
    fontSize: FontSize.sm,
    color: Colors.textSecondary,
  },
  summaryValue: {
    fontSize: FontSize.xl,
    fontWeight: 'bold',
    color: Colors.primary,
  },
  complaintsSection: {
    paddingHorizontal: Spacing.md,
    marginBottom: Spacing.sm,
  },
  sectionTitle: {
    fontSize: FontSize.md,
    fontWeight: '600',
    color: Colors.text,
    marginBottom: Spacing.sm,
  },
  complaintCard: {
    backgroundColor: Colors.warning + '15',
    padding: Spacing.md,
    borderRadius: BorderRadius.sm,
    marginRight: Spacing.sm,
    minWidth: 120,
    borderWidth: 1,
    borderColor: Colors.warning + '30',
  },
  complaintMonth: {
    fontSize: FontSize.md,
    fontWeight: 'bold',
    color: Colors.text,
  },
  complaintType: {
    fontSize: FontSize.xs,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  complaintStatus: {
    fontSize: FontSize.xs,
    fontWeight: '600',
    marginTop: 4,
  },
  listContent: {
    padding: Spacing.md,
  },
  card: {
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
  cardHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: Spacing.md,
  },
  monthBadge: {
    backgroundColor: Colors.primary,
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.xs,
    borderRadius: BorderRadius.sm,
  },
  monthText: {
    color: 'white',
    fontWeight: 'bold',
    fontSize: FontSize.sm,
  },
  statusBadge: {
    paddingHorizontal: Spacing.sm,
    paddingVertical: Spacing.xs,
    borderRadius: BorderRadius.sm,
  },
  statusText: {
    fontSize: FontSize.xs,
    fontWeight: '600',
  },
  cardBody: {
    borderTopWidth: 1,
    borderTopColor: Colors.border,
    paddingTop: Spacing.md,
  },
  salaryRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: Spacing.xs,
  },
  label: {
    fontSize: FontSize.sm,
    color: Colors.textSecondary,
  },
  value: {
    fontSize: FontSize.sm,
    color: Colors.text,
    fontWeight: '500',
  },
  netRow: {
    marginTop: Spacing.sm,
    paddingTop: Spacing.sm,
    borderTopWidth: 1,
    borderTopColor: Colors.border,
  },
  netLabel: {
    fontSize: FontSize.md,
    fontWeight: 'bold',
    color: Colors.text,
  },
  netValue: {
    fontSize: FontSize.lg,
    fontWeight: 'bold',
    color: Colors.primary,
  },
  complaintBtn: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    marginTop: Spacing.md,
    paddingVertical: Spacing.sm,
    borderTopWidth: 1,
    borderTopColor: Colors.border,
  },
  complaintBtnText: {
    color: Colors.warning,
    fontWeight: '600',
    marginLeft: Spacing.xs,
  },
  emptyContainer: {
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: Spacing.xl * 2,
  },
  emptyText: {
    fontSize: FontSize.md,
    color: Colors.textSecondary,
    marginTop: Spacing.md,
  },
  modalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0,0,0,0.5)',
    justifyContent: 'flex-end',
  },
  modalContent: {
    backgroundColor: 'white',
    borderTopLeftRadius: BorderRadius.lg,
    borderTopRightRadius: BorderRadius.lg,
    maxHeight: '85%',
  },
  modalHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: Spacing.lg,
    borderBottomWidth: 1,
    borderBottomColor: Colors.border,
  },
  modalTitle: {
    fontSize: FontSize.lg,
    fontWeight: 'bold',
    color: Colors.text,
  },
  modalBody: {
    padding: Spacing.lg,
  },
  detailSection: {
    fontSize: FontSize.sm,
    fontWeight: 'bold',
    color: Colors.primary,
    marginTop: Spacing.md,
    marginBottom: Spacing.sm,
  },
  detailRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingVertical: Spacing.xs,
  },
  detailLabel: {
    fontSize: FontSize.sm,
    color: Colors.textSecondary,
  },
  detailValue: {
    fontSize: FontSize.sm,
    fontWeight: '500',
    color: Colors.text,
  },
  netSection: {
    backgroundColor: Colors.primary + '10',
    padding: Spacing.lg,
    borderRadius: BorderRadius.md,
    alignItems: 'center',
    marginTop: Spacing.lg,
  },
  bigNetValue: {
    fontSize: FontSize.xl * 1.2,
    fontWeight: 'bold',
    color: Colors.primary,
    marginTop: Spacing.xs,
  },
  paidInfo: {
    textAlign: 'center',
    color: Colors.success,
    fontSize: FontSize.sm,
    marginTop: Spacing.md,
  },
  complaintPeriod: {
    fontSize: FontSize.md,
    fontWeight: '600',
    color: Colors.text,
    marginBottom: Spacing.md,
  },
  inputLabel: {
    fontSize: FontSize.sm,
    fontWeight: '600',
    color: Colors.text,
    marginBottom: Spacing.xs,
    marginTop: Spacing.md,
  },
  typeButtons: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: Spacing.sm,
  },
  typeBtn: {
    paddingHorizontal: Spacing.md,
    paddingVertical: Spacing.sm,
    borderRadius: BorderRadius.sm,
    backgroundColor: Colors.background,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  typeBtnActive: {
    backgroundColor: Colors.primary,
    borderColor: Colors.primary,
  },
  typeBtnText: {
    fontSize: FontSize.sm,
    color: Colors.textSecondary,
  },
  typeBtnTextActive: {
    color: 'white',
    fontWeight: '600',
  },
  textArea: {
    borderWidth: 1,
    borderColor: Colors.border,
    borderRadius: BorderRadius.md,
    padding: Spacing.md,
    fontSize: FontSize.md,
    minHeight: 100,
    textAlignVertical: 'top',
  },
  submitBtn: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: Colors.primary,
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    marginTop: Spacing.lg,
  },
  submitBtnDisabled: {
    opacity: 0.7,
  },
  submitBtnText: {
    color: 'white',
    fontWeight: 'bold',
    fontSize: FontSize.md,
    marginLeft: Spacing.sm,
  },
  actionRow: {
    flexDirection: 'row',
    gap: Spacing.sm,
    marginTop: Spacing.md,
  },
  actionBtn: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: Spacing.sm,
    borderRadius: BorderRadius.sm,
    gap: Spacing.xs,
  },
  confirmBtn: {
    backgroundColor: Colors.success,
  },
  complainBtn: {
    backgroundColor: Colors.warning,
  },
  actionBtnText: {
    color: 'white',
    fontSize: FontSize.sm,
    fontWeight: '600',
  },
});
