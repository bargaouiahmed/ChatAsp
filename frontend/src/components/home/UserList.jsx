import axiosInstance from "../../services/AxiosProvider";
import signalRService from "../../services/SignalRService";
import { useState, useEffect, useCallback } from "react";
import ChatModal from "./ChatModal";
import './UserList.css';

export default function UserList() {
    const [users, setUsers] = useState([]);
    const [selectedUser, setSelectedUser] = useState(null);
    const [searchQuery, setSearchQuery] = useState('');

    const handleUserRegistered = useCallback((newUser) => {
        setUsers(prev => {
            const exists = prev.some(u => u.id === newUser.id);
            if (exists) return prev;
            return [...prev, newUser];
        });
    }, []);

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const usersData = await axiosInstance.listUsers();
                setUsers(usersData);
            } catch (error) {
                console.error('Failed to fetch users:', error);
            }
        };
        fetchUsers();
    }, []);

    useEffect(() => {
        const setupSignalR = async () => {
            try {
                if (!signalRService.isConnected) {
                    const token = localStorage.getItem('authToken');
                    if (token) {
                        await signalRService.connect(token);
                        await new Promise(resolve => setTimeout(resolve, 200));
                    }
                }

                signalRService.off('UserRegistered');
                signalRService.on('UserRegistered', handleUserRegistered);
            } catch (error) {
                console.error('Failed to setup SignalR for user list:', error);
            }
        };

        setupSignalR();

        return () => {
            signalRService.off('UserRegistered');
        };
    }, [handleUserRegistered]);

    const filteredUsers = users.filter(user => 
        user.username.toLowerCase().includes(searchQuery.toLowerCase())
    );

    return (
        <div className="user-list-container">
            <div className="search-container">
                <input
                    type="text"
                    placeholder="Search users..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="search-input"
                />
                <svg className="search-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                    <circle cx="11" cy="11" r="8" strokeWidth="2"/>
                    <path d="M21 21l-4.35-4.35" strokeWidth="2" strokeLinecap="round"/>
                </svg>
            </div>

            <div className="users-grid">
                {filteredUsers.length === 0 ? (
                    <div className="no-users">
                        <p>No users found</p>
                    </div>
                ) : (
                    filteredUsers.map(user => (
                        <div key={user.id} className="user-card">
                            <div className="user-avatar">
                                {user.username.charAt(0).toUpperCase()}
                            </div>
                            <div className="user-info">
                                <h3>{user.username}</h3>
                            </div>
                            <button 
                                className="chat-button"
                                onClick={() => setSelectedUser(user)}
                            >
                                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                    <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                                </svg>
                                Chat
                            </button>
                        </div>
                    ))
                )}
            </div>

            {selectedUser && (
                <ChatModal
                    userId={selectedUser.id}
                    username={selectedUser.username}
                    onClose={() => setSelectedUser(null)}
                />
            )}
        </div>
    );
}
