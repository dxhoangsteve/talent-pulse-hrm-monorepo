import React, { useState, useEffect, useCallback } from 'react';
import { View, Text, StyleSheet, ScrollView, TouchableOpacity, TextInput, RefreshControl } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { 
  ArrowLeft, 
  Calendar, 
  Clock,
  MapPin,
  AlertTriangle,
  Search
} from 'lucide-react-native';

import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import { RootStackParamList } from '../types/types';
import { attendanceService, AttendanceVm } from '../services/attendanceService';
import { useAuth } from '../context/AuthContext';

type Props = NativeStackScreenProps<RootStackParamList, 'AttendanceHistory'>;

export default function AttendanceHistoryScreen({ navigation }: Props) {
  const { isAdmin } = useAuth();
  const [attendances, setAttendances] = useState<AttendanceVm[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [selectedDate, setSelectedDate] = useState(new Date().toISOString().split('T')[0]);

  const loadData = useCallback(async () => {
    try {
      let result;
      if (isAdmin) {
        result = await attendanceService.getAllAttendance(selectedDate);
      } else {
        // Manager/Deputy uses department endpoint - need to pass dept ID from context
        result = await attendanceService.getAllAttendance(selectedDate);
      }
      
      if (result.isSuccessed && result.resultObj) {
        setAttendances(result.resultObj);
      }
    } catch (error) {
      console.error('Error loading attendance:', error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, [selectedDate, isAdmin]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const formatTime = (time?: string) => {
    if (!time) return '--:--';
    return time.substring(0, 5);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Present': return Colors.success;
      case 'Late': return Colors.warning;
      case 'EarlyLeave': return Colors.warning;
      case 'Absent': return Colors.error;
      default: return Colors.textSecondary;
    }
  };

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()}>
          <ArrowLeft size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Lịch sử chấm công</Text>
        <View style={{ width: 24 }} />
      </View>

      {/* Date Filter */}
      <View style={styles.filterContainer}>
        <Calendar size={20} color={Colors.primary} />
        <TextInput
          style={styles.dateInput}
          value={selectedDate}
          onChangeText={setSelectedDate}
          placeholder="yyyy-mm-dd"
        />
        <TouchableOpacity style={styles.searchBtn} onPress={loadData}>
          <Search size={20} color="white" />
        </TouchableOpacity>
      </View>

      {/* Stats */}
      <View style={styles.statsRow}>
        <View style={styles.statItem}>
          <Text style={styles.statValue}>{attendances.length}</Text>
          <Text style={styles.statLabel}>Tổng</Text>
        </View>
        <View style={styles.statItem}>
          <Text style={[styles.statValue, { color: Colors.success }]}>
            {attendances.filter(a => a.status === 'Present').length}
          </Text>
          <Text style={styles.statLabel}>Đúng giờ</Text>
        </View>
        <View style={styles.statItem}>
          <Text style={[styles.statValue, { color: Colors.warning }]}>
            {attendances.filter(a => a.status === 'Late').length}
          </Text>
          <Text style={styles.statLabel}>Đi muộn</Text>
        </View>
        <View style={styles.statItem}>
          <Text style={[styles.statValue, { color: Colors.error }]}>
            {attendances.filter(a => a.isMockedLocation).length}
          </Text>
          <Text style={styles.statLabel}>Mocked</Text>
        </View>
      </View>

      {/* Attendance List */}
      <ScrollView 
        style={styles.content}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={() => { setRefreshing(true); loadData(); }} />
        }
      >
        {attendances.length === 0 ? (
          <View style={styles.emptyState}>
            <Clock size={48} color={Colors.textSecondary} />
            <Text style={styles.emptyText}>Không có dữ liệu chấm công</Text>
          </View>
        ) : (
          attendances.map((item) => (
            <View key={item.id} style={styles.attendanceCard}>
              <View style={styles.cardHeader}>
                <Text style={styles.employeeName}>{item.employeeName}</Text>
                <View style={[styles.statusBadge, { backgroundColor: getStatusColor(item.status) }]}>
                  <Text style={styles.statusText}>{item.statusName}</Text>
                </View>
              </View>

              <View style={styles.timeRow}>
                <View style={styles.timeBlock}>
                  <Text style={styles.timeLabel}>Vào</Text>
                  <Text style={styles.timeValue}>{formatTime(item.checkInTime)}</Text>
                </View>
                <View style={styles.timeDivider} />
                <View style={styles.timeBlock}>
                  <Text style={styles.timeLabel}>Ra</Text>
                  <Text style={styles.timeValue}>{formatTime(item.checkOutTime)}</Text>
                </View>
                <View style={styles.timeDivider} />
                <View style={styles.timeBlock}>
                  <Text style={styles.timeLabel}>Giờ làm</Text>
                  <Text style={styles.timeValue}>{item.workHours.toFixed(1)}h</Text>
                </View>
              </View>

              {/* GPS and Mocked Location Warning */}
              <View style={styles.gpsRow}>
                <MapPin size={14} color={Colors.textSecondary} />
                <Text style={styles.gpsText}>
                  {item.checkInAccuracy ? `±${Math.round(item.checkInAccuracy)}m` : 'N/A'}
                </Text>
                {item.isMockedLocation && (
                  <View style={styles.mockedWarning}>
                    <AlertTriangle size={14} color={Colors.error} />
                    <Text style={styles.mockedText}>Mocked</Text>
                  </View>
                )}
              </View>

              {item.overtimeHours > 0 && (
                <Text style={styles.otText}>OT: {item.overtimeHours.toFixed(1)} giờ</Text>
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
    padding: Spacing.md,
    backgroundColor: Colors.card,
  },
  headerTitle: { fontSize: FontSize.lg, fontWeight: '600', color: Colors.text },
  filterContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.card,
    margin: Spacing.md,
    padding: Spacing.sm,
    borderRadius: BorderRadius.md,
    gap: Spacing.sm,
  },
  dateInput: {
    flex: 1,
    backgroundColor: Colors.background,
    padding: Spacing.sm,
    borderRadius: BorderRadius.sm,
    fontSize: FontSize.md,
  },
  searchBtn: {
    backgroundColor: Colors.primary,
    padding: Spacing.sm,
    borderRadius: BorderRadius.sm,
  },
  statsRow: {
    flexDirection: 'row',
    backgroundColor: Colors.card,
    marginHorizontal: Spacing.md,
    marginBottom: Spacing.md,
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
  },
  statItem: { flex: 1, alignItems: 'center' },
  statValue: { fontSize: FontSize.xl, fontWeight: 'bold', color: Colors.text },
  statLabel: { fontSize: FontSize.xs, color: Colors.textSecondary, marginTop: 2 },
  content: { flex: 1, paddingHorizontal: Spacing.md },
  emptyState: { alignItems: 'center', marginTop: 60 },
  emptyText: { color: Colors.textSecondary, marginTop: Spacing.md },
  attendanceCard: {
    backgroundColor: Colors.card,
    borderRadius: BorderRadius.md,
    padding: Spacing.md,
    marginBottom: Spacing.sm,
  },
  cardHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: Spacing.sm,
  },
  employeeName: { fontSize: FontSize.md, fontWeight: '600', color: Colors.text },
  statusBadge: {
    paddingHorizontal: Spacing.sm,
    paddingVertical: 4,
    borderRadius: BorderRadius.full,
  },
  statusText: { color: 'white', fontSize: FontSize.xs, fontWeight: '600' },
  timeRow: {
    flexDirection: 'row',
    backgroundColor: Colors.background,
    borderRadius: BorderRadius.sm,
    padding: Spacing.sm,
  },
  timeBlock: { flex: 1, alignItems: 'center' },
  timeLabel: { fontSize: FontSize.xs, color: Colors.textSecondary },
  timeValue: { fontSize: FontSize.md, fontWeight: '600', color: Colors.text },
  timeDivider: { width: 1, backgroundColor: Colors.border },
  gpsRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: Spacing.sm,
    gap: 4,
  },
  gpsText: { fontSize: FontSize.xs, color: Colors.textSecondary },
  mockedWarning: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.error + '15',
    paddingHorizontal: Spacing.xs,
    paddingVertical: 2,
    borderRadius: BorderRadius.sm,
    marginLeft: Spacing.sm,
    gap: 4,
  },
  mockedText: { fontSize: FontSize.xs, color: Colors.error, fontWeight: '500' },
  otText: {
    fontSize: FontSize.sm,
    color: Colors.secondary,
    marginTop: Spacing.sm,
    fontWeight: '500',
  },
});
