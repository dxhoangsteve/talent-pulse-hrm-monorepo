
import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, FlatList, TouchableOpacity, ActivityIndicator, Alert, TextInput } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import { ArrowLeft, Search, UserMinus, UserPlus, Users } from 'lucide-react-native';
import { useNavigation } from '@react-navigation/native';
import { useAuth } from '../context/AuthContext';
import { departmentService, Department, UserOption } from '../services/departmentService';

export default function DepartmentEmployeeScreen() {
    const navigation = useNavigation();
    const { user } = useAuth();
    const [myDept, setMyDept] = useState<Department | null>(null);
    const [employees, setEmployees] = useState<UserOption[]>([]);
    const [loading, setLoading] = useState(true);

    // Search state
    const [searchQuery, setSearchQuery] = useState('');

    useEffect(() => {
        loadMyDepartment();
    }, []);

    const loadMyDepartment = async () => {
        setLoading(true);
        try {
            const depts = await departmentService.getDepartments();
            // Find department where current user is Manager or Deputy
            const found = depts.find(d => d.managerId === user?.nameid || d.deputyId === user?.nameid);

            if (found) {
                setMyDept(found);
                await loadEmployees(found.id);
            } else {
                // Not a manager of any dept?
                Alert.alert('Thông báo', 'Bạn không quản lý phòng ban nào.');
            }
        } catch (error) {
            console.error('Load dept error:', error);
            Alert.alert('Lỗi', 'Không thể tải thông tin phòng ban');
        } finally {
            setLoading(false);
        }
    };

    const loadEmployees = async (deptId: string) => {
        try {
            const result = await departmentService.getDepartmentEmployees(deptId);
            if (result.isSuccessed && result.resultObj) {
                setEmployees(result.resultObj);
            }
        } catch (error) {
            console.error('Load employees error:', error);
        }
    };

    const filteredEmployees = employees.filter(e =>
        e.fullName.toLowerCase().includes(searchQuery.toLowerCase()) ||
        (e.email?.toLowerCase() || '').includes(searchQuery.toLowerCase())
    );

    const renderItem = ({ item }: { item: UserOption }) => (
        <View style={styles.card}>
            <View style={styles.avatar}>
                <Text style={styles.avatarText}>{item.fullName.charAt(0)}</Text>
            </View>
            <View style={styles.info}>
                <Text style={styles.name}>{item.fullName}</Text>
                <Text style={styles.email}>{item.email}</Text>
            </View>
        </View>
    );

    return (
        <SafeAreaView style={styles.container}>
            {/* Header */}
            <View style={styles.header}>
                <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backBtn}>
                    <ArrowLeft size={24} color={Colors.text} />
                </TouchableOpacity>
                <View>
                    <Text style={styles.title}>Nhân viên Phòng ban</Text>
                    {myDept && <Text style={styles.subtitle}>{myDept.name}</Text>}
                </View>
                <View style={{ width: 24 }} />
            </View>

            {/* Search */}
            <View style={styles.searchContainer}>
                <Search size={20} color={Colors.textSecondary} />
                <TextInput
                    style={styles.searchInput}
                    placeholder="Tìm kiếm nhân viên..."
                    value={searchQuery}
                    onChangeText={setSearchQuery}
                />
            </View>

            {/* Content */}
            {loading ? (
                <ActivityIndicator size="large" color={Colors.primary} style={styles.loader} />
            ) : !myDept ? (
                <View style={styles.center}>
                    <Text style={styles.emptyText}>Bạn không quản lý phòng ban nào</Text>
                </View>
            ) : (
                <FlatList
                    data={filteredEmployees}
                    keyExtractor={item => item.id}
                    renderItem={renderItem}
                    contentContainerStyle={styles.list}
                    ListEmptyComponent={
                        <View style={styles.center}>
                            <Users size={48} color={Colors.textLight} />
                            <Text style={styles.emptyText}>Chưa có nhân viên nào</Text>
                        </View>
                    }
                />
            )}
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
    title: { fontSize: FontSize.lg, fontWeight: 'bold', color: Colors.text },
    subtitle: { fontSize: FontSize.sm, color: Colors.primary, marginTop: 2 },

    searchContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'white',
        margin: Spacing.md,
        padding: Spacing.md,
        borderRadius: BorderRadius.md,
        gap: Spacing.sm,
        borderWidth: 1,
        borderColor: Colors.border,
    },
    searchInput: { flex: 1, fontSize: FontSize.md },

    list: { padding: Spacing.md },
    card: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'white',
        padding: Spacing.md,
        borderRadius: BorderRadius.md,
        marginBottom: Spacing.sm,
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 1 },
        shadowOpacity: 0.05,
        shadowRadius: 2,
        elevation: 2,
    },
    avatar: {
        width: 40,
        height: 40,
        borderRadius: 20,
        backgroundColor: Colors.primaryLight,
        alignItems: 'center',
        justifyContent: 'center',
        marginRight: Spacing.md,
    },
    avatarText: { color: 'white', fontWeight: 'bold', fontSize: FontSize.md },
    info: { flex: 1 },
    name: { fontSize: FontSize.md, fontWeight: '600', color: Colors.text },
    email: { fontSize: FontSize.sm, color: Colors.textSecondary },

    loader: { marginTop: 40 },
    center: { alignItems: 'center', marginTop: 60 },
    emptyText: { color: Colors.textSecondary, fontSize: FontSize.md, marginTop: Spacing.md },
});
