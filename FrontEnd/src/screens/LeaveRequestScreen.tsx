import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, ScrollView, TouchableOpacity, TextInput, Alert, ActivityIndicator } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import { ArrowLeft, Plus, Calendar, X, Check, Clock } from 'lucide-react-native';
import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList, LeaveRequestItem, LeaveType, RequestStatus, getStatusLabel, getLeaveTypeLabel } from '../types/types';
import { leaveRequestService } from '../services/leaveRequestService';

type Props = NativeStackScreenProps<RootStackParamList, 'LeaveRequest'>;

export default function LeaveRequestScreen({ navigation }: Props) {
  const [requests, setRequests] = useState<LeaveRequestItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  // Form state
  const [leaveType, setLeaveType] = useState<LeaveType>(LeaveType.Annual);
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [reason, setReason] = useState('');

  useEffect(() => {
    loadRequests();
  }, []);

  const loadRequests = async () => {
    setLoading(true);
    const result = await leaveRequestService.getMyRequests();
    if (result.isSuccessed && result.resultObj) {
      setRequests(result.resultObj);
    }
    setLoading(false);
  };

  const handleSubmit = async () => {
    if (!startDate || !endDate) {
      Alert.alert('Lỗi', 'Vui lòng nhập đầy đủ ngày bắt đầu và kết thúc');
      return;
    }

    setSubmitting(true);
    const result = await leaveRequestService.createRequest({
      leaveType,
      startDate,
      endDate,
      reason: reason || undefined,
    });

    if (result.isSuccessed) {
      Alert.alert('Thành công', 'Đơn nghỉ phép đã được tạo');
      setShowForm(false);
      resetForm();
      loadRequests();
    } else {
      Alert.alert('Lỗi', result.message || 'Không thể tạo đơn');
    }
    setSubmitting(false);
  };

  const handleCancel = async (id: string) => {
    Alert.alert('Xác nhận', 'Bạn có chắc muốn hủy đơn này?', [
      { text: 'Không', style: 'cancel' },
      {
        text: 'Có',
        style: 'destructive',
        onPress: async () => {
          const result = await leaveRequestService.cancelRequest(id);
          if (result.isSuccessed) {
            loadRequests();
          } else {
            Alert.alert('Lỗi', result.message || 'Không thể hủy đơn');
          }
        },
      },
    ]);
  };

  const resetForm = () => {
    setLeaveType(LeaveType.Annual);
    setStartDate('');
    setEndDate('');
    setReason('');
  };

  const getStatusColor = (status: RequestStatus) => {
    switch (status) {
      case RequestStatus.Approved: return Colors.success;
      case RequestStatus.Rejected: return Colors.error;
      case RequestStatus.Cancelled: return Colors.textLight;
      default: return Colors.warning;
    }
  };

  const leaveTypes = [
    { value: LeaveType.Annual, label: 'Nghỉ phép năm' },
    { value: LeaveType.Sick, label: 'Nghỉ ốm' },
    { value: LeaveType.Unpaid, label: 'Không lương' },
    { value: LeaveType.Compensatory, label: 'Nghỉ bù' },
    { value: LeaveType.Other, label: 'Khác' },
  ];

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backBtn}>
          <ArrowLeft size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Xin nghỉ phép</Text>
        <TouchableOpacity onPress={() => setShowForm(true)} style={styles.addBtn}>
          <Plus size={24} color={Colors.primary} />
        </TouchableOpacity>
      </View>

      {/* Form Modal */}
      {showForm && (
        <View style={styles.formOverlay}>
          <View style={styles.formContainer}>
            <View style={styles.formHeader}>
              <Text style={styles.formTitle}>Tạo đơn nghỉ phép</Text>
              <TouchableOpacity onPress={() => setShowForm(false)}>
                <X size={24} color={Colors.text} />
              </TouchableOpacity>
            </View>

            <ScrollView style={styles.formContent}>
              {/* Leave Type */}
              <Text style={styles.inputLabel}>Loại nghỉ phép</Text>
              <View style={styles.typeContainer}>
                {leaveTypes.map((type) => (
                  <TouchableOpacity
                    key={type.value}
                    style={[styles.typeBtn, leaveType === type.value && styles.typeBtnActive]}
                    onPress={() => setLeaveType(type.value)}
                  >
                    <Text style={[styles.typeBtnText, leaveType === type.value && styles.typeBtnTextActive]}>
                      {type.label}
                    </Text>
                  </TouchableOpacity>
                ))}
              </View>

              {/* Dates */}
              <Text style={styles.inputLabel}>Ngày bắt đầu (yyyy-mm-dd)</Text>
              <TextInput
                style={styles.input}
                placeholder="2024-12-15"
                value={startDate}
                onChangeText={setStartDate}
              />

              <Text style={styles.inputLabel}>Ngày kết thúc (yyyy-mm-dd)</Text>
              <TextInput
                style={styles.input}
                placeholder="2024-12-16"
                value={endDate}
                onChangeText={setEndDate}
              />

              {/* Reason */}
              <Text style={styles.inputLabel}>Lý do (không bắt buộc)</Text>
              <TextInput
                style={[styles.input, styles.textArea]}
                placeholder="Nhập lý do xin nghỉ..."
                value={reason}
                onChangeText={setReason}
                multiline
                numberOfLines={3}
              />
            </ScrollView>

            <TouchableOpacity
              style={[styles.submitBtn, submitting && styles.submitBtnDisabled]}
              onPress={handleSubmit}
              disabled={submitting}
            >
              {submitting ? (
                <ActivityIndicator color="white" />
              ) : (
                <Text style={styles.submitBtnText}>Gửi đơn</Text>
              )}
            </TouchableOpacity>
          </View>
        </View>
      )}

      {/* Request List */}
      <ScrollView contentContainerStyle={styles.content}>
        {loading ? (
          <ActivityIndicator size="large" color={Colors.primary} style={{ marginTop: 40 }} />
        ) : requests.length === 0 ? (
          <View style={styles.emptyState}>
            <Calendar size={48} color={Colors.textLight} />
            <Text style={styles.emptyText}>Chưa có đơn nghỉ phép nào</Text>
            <TouchableOpacity style={styles.createBtn} onPress={() => setShowForm(true)}>
              <Text style={styles.createBtnText}>Tạo đơn mới</Text>
            </TouchableOpacity>
          </View>
        ) : (
          requests.map((item) => (
            <View key={item.id} style={styles.requestCard}>
              <View style={styles.cardHeader}>
                <View style={[styles.statusBadge, { backgroundColor: getStatusColor(item.status) }]}>
                  <Text style={styles.statusText}>{getStatusLabel(item.status)}</Text>
                </View>
                <Text style={styles.leaveTypeText}>{getLeaveTypeLabel(item.leaveType)}</Text>
              </View>

              <View style={styles.cardBody}>
                <View style={styles.dateRow}>
                  <Calendar size={16} color={Colors.textSecondary} />
                  <Text style={styles.dateText}>
                    {new Date(item.startDate).toLocaleDateString('vi-VN')} - {new Date(item.endDate).toLocaleDateString('vi-VN')}
                  </Text>
                </View>
                <Text style={styles.totalDays}>{item.totalDays} ngày</Text>
              </View>

              {item.approvedByName && (
                <Text style={styles.approverText}>Duyệt bởi: {item.approvedByName}</Text>
              )}

              {item.status === RequestStatus.Pending && (
                <TouchableOpacity style={styles.cancelBtn} onPress={() => handleCancel(item.id)}>
                  <Text style={styles.cancelBtnText}>Hủy đơn</Text>
                </TouchableOpacity>
              )}
            </View>
          ))
        )}
      </ScrollView>
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
  addBtn: { padding: Spacing.xs },
  content: { padding: Spacing.lg },

  // Form
  formOverlay: {
    ...StyleSheet.absoluteFillObject,
    backgroundColor: 'rgba(0,0,0,0.5)',
    justifyContent: 'flex-end',
    zIndex: 100,
  },
  formContainer: {
    backgroundColor: 'white',
    borderTopLeftRadius: BorderRadius.lg,
    borderTopRightRadius: BorderRadius.lg,
    maxHeight: '80%',
  },
  formHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: Spacing.lg,
    borderBottomWidth: 1,
    borderBottomColor: Colors.border,
  },
  formTitle: { fontSize: FontSize.lg, fontWeight: 'bold', color: Colors.text },
  formContent: { padding: Spacing.lg },
  inputLabel: { fontSize: FontSize.sm, fontWeight: '600', color: Colors.textSecondary, marginBottom: Spacing.xs, marginTop: Spacing.md },
  input: {
    backgroundColor: Colors.background,
    borderRadius: BorderRadius.sm,
    padding: Spacing.md,
    fontSize: FontSize.md,
    borderWidth: 1,
    borderColor: Colors.border,
  },
  textArea: { height: 80, textAlignVertical: 'top' },
  typeContainer: { flexDirection: 'row', flexWrap: 'wrap', gap: Spacing.sm },
  typeBtn: {
    paddingVertical: Spacing.sm,
    paddingHorizontal: Spacing.md,
    borderRadius: BorderRadius.full,
    borderWidth: 1,
    borderColor: Colors.border,
    backgroundColor: 'white',
  },
  typeBtnActive: { backgroundColor: Colors.primary, borderColor: Colors.primary },
  typeBtnText: { fontSize: FontSize.sm, color: Colors.text },
  typeBtnTextActive: { color: 'white' },
  submitBtn: {
    backgroundColor: Colors.primary,
    margin: Spacing.lg,
    padding: Spacing.md,
    borderRadius: BorderRadius.sm,
    alignItems: 'center',
  },
  submitBtnDisabled: { backgroundColor: Colors.primaryLight },
  submitBtnText: { color: 'white', fontWeight: 'bold', fontSize: FontSize.md },

  // Empty state
  emptyState: { alignItems: 'center', marginTop: 60 },
  emptyText: { color: Colors.textSecondary, fontSize: FontSize.md, marginTop: Spacing.md },
  createBtn: {
    marginTop: Spacing.lg,
    backgroundColor: Colors.primary,
    paddingVertical: Spacing.md,
    paddingHorizontal: Spacing.xl,
    borderRadius: BorderRadius.sm,
  },
  createBtnText: { color: 'white', fontWeight: 'bold' },

  // Request card
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
  cardHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: Spacing.sm },
  statusBadge: { paddingHorizontal: Spacing.sm, paddingVertical: 4, borderRadius: BorderRadius.full },
  statusText: { color: 'white', fontSize: FontSize.xs, fontWeight: '600' },
  leaveTypeText: { fontSize: FontSize.sm, color: Colors.text, fontWeight: '600' },
  cardBody: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
  dateRow: { flexDirection: 'row', alignItems: 'center', gap: Spacing.xs },
  dateText: { color: Colors.textSecondary, fontSize: FontSize.sm },
  totalDays: { fontSize: FontSize.lg, fontWeight: 'bold', color: Colors.primary },
  approverText: { fontSize: FontSize.xs, color: Colors.textSecondary, marginTop: Spacing.sm },
  cancelBtn: {
    marginTop: Spacing.sm,
    padding: Spacing.sm,
    borderRadius: BorderRadius.sm,
    backgroundColor: Colors.background,
    alignItems: 'center',
  },
  cancelBtnText: { color: Colors.error, fontWeight: '600', fontSize: FontSize.sm },
});
