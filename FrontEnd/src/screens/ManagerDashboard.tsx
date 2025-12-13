import React from 'react';
import { View, Text, StyleSheet, ScrollView, TouchableOpacity } from 'react-native';
import { useAuth } from '../context/AuthContext';
import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import { 
  Users, 
  DollarSign, 
  Calendar, 
  LogOut,
  Bell,
  ChevronRight,
  Eye
} from 'lucide-react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../types/types';

type Props = NativeStackScreenProps<RootStackParamList, 'ManagerDashboard'>;

export default function ManagerDashboard({ navigation }: Props) {
  const { user, logout } = useAuth();

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <View>
          <Text style={styles.welcomeText}>Xin chào,</Text>
          <Text style={styles.userName}>{user?.name || 'Trưởng phòng'}</Text>
          <Text style={styles.roleText}>Quản lý phòng ban</Text>
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
        {/* Quick Actions */}
        <Text style={styles.sectionTitle}>Quản lý phòng ban</Text>
        <View style={styles.actionList}>
          <ActionItem 
            title="Duyệt đơn nghỉ phép & OT" 
            subtitle="Phê duyệt yêu cầu của nhân viên"
            icon={<Calendar size={24} color="white" />}
            color={Colors.secondary}
            onPress={() => navigation.navigate('ApprovalScreen')}
          />
          <ActionItem 
            title="Xem nhân viên" 
            subtitle="Danh sách nhân viên trong phòng ban"
            icon={<Users size={24} color="white" />}
            color={Colors.primary}
            onPress={() => navigation.navigate('DepartmentEmployees')}
          />
          <ActionItem 
            title="Xem lương phòng ban" 
            subtitle="Lương nhân viên trong phòng (chỉ xem)"
            icon={<DollarSign size={24} color="white" />}
            color={Colors.warning}
            onPress={() => navigation.navigate('DepartmentSalary')}
          />
        </View>

        {/* Info Notice */}
        <View style={styles.infoBox}>
          <Eye size={20} color={Colors.secondary} />
          <Text style={styles.infoText}>
            Bạn chỉ có thể xem và duyệt đơn của nhân viên trong phòng ban mình quản lý.
          </Text>
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}

const ActionItem = ({ title, subtitle, icon, color, onPress }: any) => (
  <TouchableOpacity style={styles.actionItem} onPress={onPress}>
    <View style={[styles.actionIcon, { backgroundColor: color }]}>
      {icon}
    </View>
    <View style={styles.actionContent}>
      <Text style={styles.actionTitle}>{title}</Text>
      <Text style={styles.actionSubtitle}>{subtitle}</Text>
    </View>
    <ChevronRight size={20} color={Colors.textSecondary} />
  </TouchableOpacity>
);

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    padding: Spacing.lg,
    backgroundColor: Colors.card,
  },
  welcomeText: {
    fontSize: FontSize.sm,
    color: Colors.textSecondary,
  },
  userName: {
    fontSize: FontSize.xl,
    fontWeight: 'bold',
    color: Colors.text,
    marginTop: 2,
  },
  roleText: {
    fontSize: FontSize.xs,
    color: Colors.secondary,
    marginTop: 4,
  },
  headerActions: {
    flexDirection: 'row',
    gap: 8,
  },
  iconButton: {
    padding: Spacing.sm,
  },
  content: {
    flex: 1,
  },
  scrollContent: {
    padding: Spacing.lg,
  },
  sectionTitle: {
    fontSize: FontSize.lg,
    fontWeight: '600',
    color: Colors.text,
    marginBottom: Spacing.md,
    marginTop: Spacing.sm,
  },
  actionList: {
    gap: Spacing.sm,
  },
  actionItem: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.card,
    padding: Spacing.md,
    borderRadius: BorderRadius.lg,
  },
  actionIcon: {
    width: 48,
    height: 48,
    borderRadius: BorderRadius.md,
    alignItems: 'center',
    justifyContent: 'center',
    marginRight: Spacing.md,
  },
  actionContent: {
    flex: 1,
  },
  actionTitle: {
    fontSize: FontSize.md,
    fontWeight: '600',
    color: Colors.text,
  },
  actionSubtitle: {
    fontSize: FontSize.sm,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  infoBox: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.secondary + '15',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    marginTop: Spacing.xl,
    gap: Spacing.sm,
  },
  infoText: {
    flex: 1,
    fontSize: FontSize.sm,
    color: Colors.secondary,
  },
});
