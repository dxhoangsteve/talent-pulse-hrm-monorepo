import React from 'react';
import { View, Text, StyleSheet, ScrollView, TouchableOpacity } from 'react-native';
import { useAuth } from '../context/AuthContext';
import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import { 
  Users, 
  Building, 
  DollarSign, 
  Calendar, 
  LogOut,
  Bell,
  Menu,
  ChevronRight,
  Clock
} from 'lucide-react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../types/types';

type Props = NativeStackScreenProps<RootStackParamList, 'AdminDashboard'>;

export default function AdminDashboard({ navigation }: Props) {
  const { user, logout } = useAuth();

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <View>
          <Text style={styles.welcomeText}>Xin chào,</Text>
          <Text style={styles.userName}>{user?.name || 'Admin'}</Text>
        </View>
        <View style={styles.headerActions}>
          <TouchableOpacity style={styles.iconButton}>
            <Bell size={24} color={Colors.text} />
          </TouchableOpacity>
          <TouchableOpacity style={styles.iconButton} onPress={logout}>
            <LogOut size={24} color={Colors.error} />
          </TouchableOpacity>
        </View>
      </View>

      <ScrollView style={styles.content} contentContainerStyle={styles.scrollContent}>
        {/* Stats Grid */}
        <Text style={styles.sectionTitle}>Tổng quan</Text>
        <View style={styles.statsGrid}>
          <StatCard 
            title="Nhân viên" 
            value="124" 
            icon={<Users size={24} color="#6366F1" />} 
            color="#EEF2FF"
          />
          <StatCard 
            title="Phòng ban" 
            value="8" 
            icon={<Building size={24} color="#10B981" />} 
            color="#ECFDF5"
          />
          <StatCard 
            title="Đơn nghỉ" 
            value="12" 
            icon={<Calendar size={24} color="#F59E0B" />} 
            color="#FFFBEB"
          />
          <StatCard 
            title="Lương tháng" 
            value="1.2T" 
            icon={<DollarSign size={24} color="#EF4444" />} 
            color="#FEF2F2"
          />
        </View>

        {/* Quick Actions */}
        <Text style={styles.sectionTitle}>Quản lý</Text>
        <View style={styles.actionList}>
          <ActionItem 
            title="Quản lý Nhân sự" 
            subtitle="Thêm, sửa, xóa nhân viên"
            icon={<Users size={24} color="white" />}
            color={Colors.primary}
            onPress={() => navigation.navigate('UserManagement')}
          />
          <ActionItem 
            title="Chấm công & Nghỉ phép" 
            subtitle="Phê duyệt yêu cầu"
            icon={<Calendar size={24} color="white" />}
            color={Colors.secondary}
            onPress={() => navigation.navigate('ApprovalScreen')}
          />
          <ActionItem 
            title="Quản lý Phòng ban" 
            subtitle="Thay đổi trưởng/phó phòng"
            icon={<Building size={24} color="white" />}
            color="#8B5CF6"
            onPress={() => navigation.navigate('DepartmentManagement')}
          />
          <ActionItem 
            title="Tính lương" 
            subtitle="Bảng lương tháng 12"
            icon={<DollarSign size={24} color="white" />}
            color={Colors.warning}
          />
          <ActionItem 
            title="Lịch sử chấm công" 
            subtitle="Xem chấm công nhân viên"
            icon={<Clock size={24} color="white" />}
            color="#10B981"
            onPress={() => navigation.navigate('AttendanceHistory')}
          />
          <ActionItem 
            title="Lịch sử Nghỉ phép" 
            subtitle="Tất cả đơn nghỉ phép"
            icon={<Calendar size={24} color="white" />}
            color="#2196F3"
            onPress={() => navigation.navigate('LeaveHistory')}
          />
          <ActionItem 
            title="Lịch sử Tăng ca" 
            subtitle="Tất cả đơn OT"
            icon={<Clock size={24} color="white" />}
            color="#FF9800"
            onPress={() => navigation.navigate('OTHistory')}
          />
          <ActionItem 
            title="Lịch sử Lương" 
            subtitle="Bảng lương & khiếu nại"
            icon={<DollarSign size={24} color="white" />}
            color="#4CAF50"
            onPress={() => navigation.navigate('SalaryHistory')}
          />
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}

const StatCard = ({ title, value, icon, color }: any) => (
  <View style={[styles.statCard, { backgroundColor: color }]}>
    <View style={styles.statIcon}>{icon}</View>
    <Text style={styles.statValue}>{value}</Text>
    <Text style={styles.statTitle}>{title}</Text>
  </View>
);

const ActionItem = ({ title, subtitle, icon, color, onPress }: any) => (
  <TouchableOpacity style={styles.actionItem} onPress={onPress}>
    <View style={[styles.actionIcon, { backgroundColor: color }]}>
      {icon}
    </View>
    <View style={styles.actionInfo}>
      <Text style={styles.actionTitle}>{title}</Text>
      <Text style={styles.actionSubtitle}>{subtitle}</Text>
    </View>
    <ChevronRight size={20} color={Colors.textLight} />
  </TouchableOpacity>
);

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  header: {
    padding: Spacing.lg,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: 'white',
    borderBottomWidth: 1,
    borderBottomColor: Colors.border,
  },
  welcomeText: {
    fontSize: FontSize.sm,
    color: Colors.textSecondary,
  },
  userName: {
    fontSize: FontSize.lg,
    fontWeight: 'bold',
    color: Colors.text,
  },
  headerActions: {
    flexDirection: 'row',
    gap: Spacing.md,
  },
  iconButton: {
    padding: Spacing.xs,
  },
  content: {
    flex: 1,
  },
  scrollContent: {
    padding: Spacing.lg,
  },
  sectionTitle: {
    fontSize: FontSize.lg,
    fontWeight: 'bold',
    color: Colors.text,
    marginBottom: Spacing.md,
    marginTop: Spacing.sm,
  },
  statsGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: Spacing.md,
    marginBottom: Spacing.xl,
  },
  statCard: {
    width: '47%',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    alignItems: 'center',
  },
  statIcon: {
    marginBottom: Spacing.sm,
  },
  statValue: {
    fontSize: FontSize.xl,
    fontWeight: 'bold',
    color: Colors.text,
  },
  statTitle: {
    fontSize: FontSize.xs,
    color: Colors.textSecondary,
  },
  actionList: {
    gap: Spacing.md,
  },
  actionItem: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: 'white',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    shadowColor: Colors.text,
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  actionIcon: {
    width: 48,
    height: 48,
    borderRadius: BorderRadius.md,
    alignItems: 'center',
    justifyContent: 'center',
    marginRight: Spacing.md,
  },
  actionInfo: {
    flex: 1,
  },
  actionTitle: {
    fontSize: FontSize.md,
    fontWeight: '600',
    color: Colors.text,
  },
  actionSubtitle: {
    fontSize: FontSize.xs,
    color: Colors.textSecondary,
  },
});
