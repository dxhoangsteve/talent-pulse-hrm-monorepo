import React, { useState, useEffect, useCallback } from 'react';
import { View, Text, StyleSheet, TouchableOpacity, Alert, ScrollView, RefreshControl } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { NativeStackScreenProps } from '@react-navigation/native-stack';
import * as Location from 'expo-location';
import { 
  MapPin, 
  Clock, 
  CheckCircle, 
  XCircle, 
  ArrowLeft,
  AlertTriangle
} from 'lucide-react-native';

import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import { RootStackParamList } from '../types/types';
import { attendanceService, TodayAttendanceVm, AttendanceVm } from '../services/attendanceService';

type Props = NativeStackScreenProps<RootStackParamList, 'AttendanceScreen'>;

export default function AttendanceScreen({ navigation }: Props) {
  const [todayStatus, setTodayStatus] = useState<TodayAttendanceVm | null>(null);
  const [history, setHistory] = useState<AttendanceVm[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [locationLoading, setLocationLoading] = useState(false);
  const [currentLocation, setCurrentLocation] = useState<Location.LocationObject | null>(null);

  const loadData = useCallback(async () => {
    try {
      const [statusRes, historyRes] = await Promise.all([
        attendanceService.getTodayStatus(),
        attendanceService.getMyAttendance(new Date().getMonth() + 1, new Date().getFullYear())
      ]);
      
      if (statusRes.isSuccessed && statusRes.resultObj) {
        setTodayStatus(statusRes.resultObj);
      }
      if (historyRes.isSuccessed && historyRes.resultObj) {
        setHistory(historyRes.resultObj);
      }
    } catch (error) {
      console.error('Error loading attendance:', error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, []);

  useEffect(() => {
    loadData();
    requestLocationPermission();
  }, [loadData]);

  const requestLocationPermission = async () => {
    const { status } = await Location.requestForegroundPermissionsAsync();
    if (status !== 'granted') {
      Alert.alert('Lỗi', 'Cần quyền truy cập vị trí để chấm công');
    }
  };

  const getCurrentLocation = async () => {
    setLocationLoading(true);
    try {
      const location = await Location.getCurrentPositionAsync({
        accuracy: Location.Accuracy.High,
      });
      setCurrentLocation(location);
      return location;
    } catch (error) {
      Alert.alert('Lỗi', 'Không thể lấy vị trí hiện tại');
      return null;
    } finally {
      setLocationLoading(false);
    }
  };

  const handleCheckIn = async () => {
    const location = await getCurrentLocation();
    if (!location) return;

    // Check if location is mocked
    const isMocked = (location as any).mocked || false;
    
    if (isMocked) {
      Alert.alert('Lỗi', 'Phát hiện vị trí giả! Không thể check-in.');
      return;
    }

    try {
      const result = await attendanceService.checkIn({
        latitude: location.coords.latitude,
        longitude: location.coords.longitude,
        accuracy: location.coords.accuracy || 0,
        isMockedLocation: isMocked,
      });

      if (result.isSuccessed) {
        Alert.alert('Thành công', 'Check-in thành công!');
        loadData();
      } else {
        Alert.alert('Lỗi', result.message || 'Check-in thất bại');
      }
    } catch (error) {
      Alert.alert('Lỗi', 'Có lỗi xảy ra khi check-in');
    }
  };

  const handleCheckOut = async () => {
    const location = await getCurrentLocation();
    if (!location) return;

    const isMocked = (location as any).mocked || false;
    
    if (isMocked) {
      Alert.alert('Lỗi', 'Phát hiện vị trí giả! Không thể check-out.');
      return;
    }

    try {
      const result = await attendanceService.checkOut({
        latitude: location.coords.latitude,
        longitude: location.coords.longitude,
        accuracy: location.coords.accuracy || 0,
        isMockedLocation: isMocked,
      });

      if (result.isSuccessed) {
        Alert.alert('Thành công', 'Check-out thành công!');
        loadData();
      } else {
        Alert.alert('Lỗi', result.message || 'Check-out thất bại');
      }
    } catch (error) {
      Alert.alert('Lỗi', 'Có lỗi xảy ra khi check-out');
    }
  };

  const formatTime = (time?: string) => {
    if (!time) return '--:--';
    // Time comes as "HH:MM:SS" format
    return time.substring(0, 5);
  };

  const formatDate = (dateStr: string) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' });
  };

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()}>
          <ArrowLeft size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Chấm công</Text>
        <View style={{ width: 24 }} />
      </View>

      <ScrollView 
        style={styles.content}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={() => { setRefreshing(true); loadData(); }} />
        }
      >
        {/* Today Status Card */}
        <View style={styles.statusCard}>
          <Text style={styles.statusTitle}>Hôm nay</Text>
          
          <View style={styles.timeRow}>
            <View style={styles.timeBlock}>
              <Text style={styles.timeLabel}>Check-in</Text>
              <Text style={styles.timeValue}>
                {todayStatus?.hasCheckedIn ? formatTime(todayStatus.checkInTime) : '--:--'}
              </Text>
              {todayStatus?.hasCheckedIn ? (
                <CheckCircle size={20} color={Colors.success} />
              ) : (
                <XCircle size={20} color={Colors.textSecondary} />
              )}
            </View>
            
            <View style={styles.timeDivider} />
            
            <View style={styles.timeBlock}>
              <Text style={styles.timeLabel}>Check-out</Text>
              <Text style={styles.timeValue}>
                {todayStatus?.hasCheckedOut ? formatTime(todayStatus.checkOutTime) : '--:--'}
              </Text>
              {todayStatus?.hasCheckedOut ? (
                <CheckCircle size={20} color={Colors.success} />
              ) : (
                <XCircle size={20} color={Colors.textSecondary} />
              )}
            </View>
          </View>

          {todayStatus && todayStatus.workHours && todayStatus.workHours > 0 && (
            <View style={styles.workHoursRow}>
              <Clock size={16} color={Colors.secondary} />
              <Text style={styles.workHoursText}>
                Đã làm: {todayStatus.workHours.toFixed(1)} giờ
              </Text>
            </View>
          )}
        </View>

        {/* Location Status */}
        <View style={styles.locationCard}>
          <MapPin size={20} color={Colors.primary} />
          <Text style={styles.locationText}>
            {currentLocation 
              ? `Độ chính xác: ${Math.round(currentLocation.coords.accuracy || 0)}m`
              : 'Chưa lấy vị trí'
            }
          </Text>
          <TouchableOpacity onPress={getCurrentLocation} disabled={locationLoading}>
            <Text style={styles.refreshLocation}>Làm mới</Text>
          </TouchableOpacity>
        </View>

        {/* Action Buttons */}
        <View style={styles.actionButtons}>
          <TouchableOpacity 
            style={[styles.checkInBtn, todayStatus?.hasCheckedIn && styles.disabledBtn]}
            onPress={handleCheckIn}
            disabled={todayStatus?.hasCheckedIn || locationLoading}
          >
            <Text style={styles.btnText}>
              {todayStatus?.hasCheckedIn ? 'Đã Check-in' : 'Check-in'}
            </Text>
          </TouchableOpacity>

          <TouchableOpacity 
            style={[styles.checkOutBtn, (!todayStatus?.hasCheckedIn || todayStatus?.hasCheckedOut) && styles.disabledBtn]}
            onPress={handleCheckOut}
            disabled={!todayStatus?.hasCheckedIn || todayStatus?.hasCheckedOut || locationLoading}
          >
            <Text style={styles.btnText}>
              {todayStatus?.hasCheckedOut ? 'Đã Check-out' : 'Check-out'}
            </Text>
          </TouchableOpacity>
        </View>

        {/* History */}
        <Text style={styles.sectionTitle}>Lịch sử tháng này</Text>
        {history.map((item) => (
          <View key={item.id} style={styles.historyItem}>
            <View style={styles.historyDate}>
              <Text style={styles.historyDateText}>{formatDate(item.date)}</Text>
            </View>
            <View style={styles.historyDetails}>
              <Text style={styles.historyTime}>
                {formatTime(item.checkInTime)} - {formatTime(item.checkOutTime)}
              </Text>
              <Text style={[
                styles.historyStatus,
                item.status === 'Present' && { color: Colors.success },
                item.status === 'Late' && { color: Colors.warning },
                item.status === 'EarlyLeave' && { color: Colors.warning },
              ]}>
                {item.statusName}
              </Text>
            </View>
            {item.isMockedLocation && (
              <AlertTriangle size={16} color={Colors.error} />
            )}
          </View>
        ))}
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
  content: { flex: 1, padding: Spacing.md },
  statusCard: {
    backgroundColor: Colors.card,
    borderRadius: BorderRadius.lg,
    padding: Spacing.lg,
    marginBottom: Spacing.md,
  },
  statusTitle: { fontSize: FontSize.lg, fontWeight: '600', color: Colors.text, marginBottom: Spacing.md },
  timeRow: { flexDirection: 'row', alignItems: 'center', justifyContent: 'center' },
  timeBlock: { alignItems: 'center', flex: 1 },
  timeLabel: { fontSize: FontSize.sm, color: Colors.textSecondary },
  timeValue: { fontSize: FontSize.xxl, fontWeight: 'bold', color: Colors.text, marginVertical: 4 },
  timeDivider: { width: 1, height: 60, backgroundColor: Colors.border },
  workHoursRow: { flexDirection: 'row', alignItems: 'center', justifyContent: 'center', marginTop: Spacing.md },
  workHoursText: { marginLeft: 8, color: Colors.secondary, fontSize: FontSize.sm },
  locationCard: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.card,
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    marginBottom: Spacing.md,
  },
  locationText: { flex: 1, marginLeft: Spacing.sm, color: Colors.text },
  refreshLocation: { color: Colors.primary, fontWeight: '500' },
  actionButtons: { gap: Spacing.sm, marginBottom: Spacing.lg },
  checkInBtn: {
    backgroundColor: Colors.success,
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    alignItems: 'center',
  },
  checkOutBtn: {
    backgroundColor: Colors.warning,
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    alignItems: 'center',
  },
  disabledBtn: { opacity: 0.5 },
  btnText: { color: 'white', fontWeight: '600', fontSize: FontSize.md },
  sectionTitle: { fontSize: FontSize.lg, fontWeight: '600', color: Colors.text, marginBottom: Spacing.md },
  historyItem: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.card,
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    marginBottom: Spacing.sm,
  },
  historyDate: {
    backgroundColor: Colors.primaryLight,
    paddingHorizontal: Spacing.sm,
    paddingVertical: 4,
    borderRadius: BorderRadius.sm,
    marginRight: Spacing.md,
  },
  historyDateText: { color: Colors.primary, fontWeight: '600' },
  historyDetails: { flex: 1 },
  historyTime: { fontSize: FontSize.md, color: Colors.text },
  historyStatus: { fontSize: FontSize.sm, color: Colors.textSecondary },
});
