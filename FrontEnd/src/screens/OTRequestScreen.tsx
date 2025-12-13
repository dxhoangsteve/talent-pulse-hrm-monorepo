import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, ScrollView, TouchableOpacity, TextInput, Alert, ActivityIndicator } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import { ArrowLeft, Plus, Clock, X } from 'lucide-react-native';
import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList, OTRequestItem, RequestStatus, getStatusLabel } from '../types/types';
import { overtimeRequestService } from '../services/overtimeRequestService';

type Props = NativeStackScreenProps<RootStackParamList, 'OTRequest'>;

export default function OTRequestScreen({ navigation }: Props) {
  const [requests, setRequests] = useState<OTRequestItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  // Form state
  const [otDate, setOtDate] = useState('');
  const [startTime, setStartTime] = useState('');
  const [endTime, setEndTime] = useState('');
  const [multiplier, setMultiplier] = useState('1.5');
  const [reason, setReason] = useState('');

  useEffect(() => {
    loadRequests();
  }, []);

  const loadRequests = async () => {
    setLoading(true);
    const result = await overtimeRequestService.getMyRequests();
    if (result.isSuccessed && result.resultObj) {
      setRequests(result.resultObj);
    }
    setLoading(false);
  };

  const handleSubmit = async () => {
    if (!otDate || !startTime || !endTime) {
      Alert.alert('Lỗi', 'Vui lòng nhập đầy đủ ngày và giờ OT');
      return;
    }

    setSubmitting(true);
    const result = await overtimeRequestService.createRequest({
      otDate,
      startTime,
      endTime,
      multiplier: parseFloat(multiplier),
      reason: reason || undefined,
    });

    if (result.isSuccessed) {
      Alert.alert('Thành công', 'Đơn OT đã được tạo');
      setShowForm(false);
      resetForm();
      loadRequests();
    } else {
      Alert.alert('Lỗi', result.message || 'Không thể tạo đơn OT');
    }
    setSubmitting(false);
  };

  const handleCancel = async (id: string) => {
    Alert.alert('Xác nhận', 'Bạn có chắc muốn hủy đơn OT này?', [
      { text: 'Không', style: 'cancel' },
      {
        text: 'Có',
        style: 'destructive',
        onPress: async () => {
          const result = await overtimeRequestService.cancelRequest(id);
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
    setOtDate('');
    setStartTime('');
    setEndTime('');
    setMultiplier('1.5');
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

  const multipliers = [
    { value: '1.5', label: '1.5x' },
    { value: '2', label: '2x (CN)' },
    { value: '3', label: '3x (Lễ)' },
  ];

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backBtn}>
          <ArrowLeft size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Đăng ký OT</Text>
        <TouchableOpacity onPress={() => setShowForm(true)} style={styles.addBtn}>
          <Plus size={24} color={Colors.primary} />
        </TouchableOpacity>
      </View>

      {/* Form Modal */}
      {showForm && (
        <View style={styles.formOverlay}>
          <View style={styles.formContainer}>
            <View style={styles.formHeader}>
              <Text style={styles.formTitle}>Tạo đơn OT</Text>
              <TouchableOpacity onPress={() => setShowForm(false)}>
                <X size={24} color={Colors.text} />
              </TouchableOpacity>
            </View>

            <ScrollView style={styles.formContent}>
              {/* OT Date */}
              <Text style={styles.inputLabel}>Ngày OT (yyyy-mm-dd)</Text>
              <TextInput
                style={styles.input}
                placeholder="2024-12-15"
                value={otDate}
                onChangeText={setOtDate}
              />

              {/* Time */}
              <Text style={styles.inputLabel}>Giờ bắt đầu (HH:mm:ss)</Text>
              <TextInput
                style={styles.input}
                placeholder="18:00:00"
                value={startTime}
                onChangeText={setStartTime}
              />

              <Text style={styles.inputLabel}>Giờ kết thúc (HH:mm:ss)</Text>
              <TextInput
                style={styles.input}
                placeholder="21:00:00"
                value={endTime}
                onChangeText={setEndTime}
              />

              {/* Multiplier */}
              <Text style={styles.inputLabel}>Hệ số OT</Text>
              <View style={styles.typeContainer}>
                {multipliers.map((m) => (
                  <TouchableOpacity
                    key={m.value}
                    style={[styles.typeBtn, multiplier === m.value && styles.typeBtnActive]}
                    onPress={() => setMultiplier(m.value)}
                  >
                    <Text style={[styles.typeBtnText, multiplier === m.value && styles.typeBtnTextActive]}>
                      {m.label}
                    </Text>
                  </TouchableOpacity>
                ))}
              </View>

              {/* Reason */}
              <Text style={styles.inputLabel}>Lý do (không bắt buộc)</Text>
              <TextInput
                style={[styles.input, styles.textArea]}
                placeholder="Nhập lý do OT..."
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
            <Clock size={48} color={Colors.textLight} />
            <Text style={styles.emptyText}>Chưa có đơn OT nào</Text>
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
                <Text style={styles.multiplierText}>x{item.multiplier}</Text>
              </View>

              <View style={styles.cardBody}>
                <View style={styles.dateRow}>
                  <Clock size={16} color={Colors.textSecondary} />
                  <Text style={styles.dateText}>
                    {new Date(item.otDate).toLocaleDateString('vi-VN')}
                  </Text>
                </View>
                <Text style={styles.hoursText}>{item.hours.toFixed(1)}h</Text>
              </View>

              <Text style={styles.timeRangeText}>
                {item.startTime.substring(0, 5)} - {item.endTime.substring(0, 5)}
              </Text>

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
  multiplierText: { fontSize: FontSize.sm, color: Colors.primary, fontWeight: 'bold' },
  cardBody: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
  dateRow: { flexDirection: 'row', alignItems: 'center', gap: Spacing.xs },
  dateText: { color: Colors.textSecondary, fontSize: FontSize.sm },
  hoursText: { fontSize: FontSize.lg, fontWeight: 'bold', color: Colors.secondary },
  timeRangeText: { fontSize: FontSize.sm, color: Colors.textSecondary, marginTop: Spacing.xs },
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
