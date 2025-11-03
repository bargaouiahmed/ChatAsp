import { useState, useEffect, useCallback } from "react";
import signalRService from "../../services/SignalRService";
import axiosInstance from "../../services/AxiosProvider";
import ChatModal from "./ChatModal";
import './ConversationsSidebar.css';

export default function ConversationsSidebar() {
    const [rooms, setRooms] = useState([]);
    const [onlineUsers, setOnlineUsers] = useState(new Set());
    const [selectedChat, setSelectedChat] = useState(null);
    const [loading, setLoading] = useState(true);
    const currentUserId = parseInt(localStorage.getItem('userId'));

    const handleUserStatusChanged = useCallback((userId, isOnline) => {
        setOnlineUsers(prev => {
            const newSet = new Set(prev);
            if (isOnline) {
                newSet.add(userId);
            } else {
                newSet.delete(userId);
            }
            return newSet;
        });
    }, []);

    useEffect(() => {
        const initializeSidebar = async () => {
            try {
                setLoading(true);
                
                if (!signalRService.isConnected) {
                    const token = localStorage.getItem('authToken');
                    await signalRService.connect(token);
                    await new Promise(resolve => setTimeout(resolve, 200));
                }

                signalRService.off('UserStatusChanged');
                signalRService.on('UserStatusChanged', handleUserStatusChanged);
                signalRService.off('RoomCreated');
                signalRService.on('RoomCreated', (room) => {
                    setRooms(prev => [...prev, room]);
                });


                const userRooms = await axiosInstance.getRoomsByUserId(currentUserId);
                setRooms(userRooms);
                
                const onlineUsersFromRooms = new Set();
                userRooms.forEach(room => {
                    if (room.userId1 === currentUserId && room.user2Connected) {
                        onlineUsersFromRooms.add(room.userId2);
                    } else if (room.userId2 === currentUserId && room.user1Connected) {
                        onlineUsersFromRooms.add(room.userId1);
                    }
                });
                setOnlineUsers(onlineUsersFromRooms);

                setLoading(false);
            } catch (err) {
                console.error('Failed to initialize sidebar:', err);
                setLoading(false);
            }
        };

        initializeSidebar();

        return () => {
            signalRService.off('UserStatusChanged');
        };
    }, [currentUserId, handleUserStatusChanged]);

    const getOtherUser = (room) => {
        if (room.userId1 === currentUserId) {
            return {
                id: room.userId2,
                username: room.user2Username
            };
        } else {
            return {
                id: room.userId1,
                username: room.user1Username
            };
        }
    };

    const isUserOnline = (userId) => {
        return onlineUsers.has(userId);
    };

    if (loading) {
        return (
            <div className="conversations-sidebar">
                <div className="sidebar-header">
                    <h2>Conversations</h2>
                </div>
                <div className="loading-rooms">
                    <div className="spinner-small"></div>
                    <p>Loading...</p>
                </div>
            </div>
        );
    }

    return (
        <>
            <div className="conversations-sidebar">
                <div className="sidebar-header">
                    <h2>Conversations</h2>
                    <span className="room-count">{rooms.length}</span>
                </div>

                <div className="rooms-list">
                    {rooms.length === 0 ? (
                        <div className="no-rooms">
                            <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                                <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" strokeWidth="2"/>
                            </svg>
                            <p>No conversations yet</p>
                            <small>Start chatting with someone!</small>
                        </div>
                    ) : (
                        rooms.map(room => {
                            const otherUser = getOtherUser(room);
                            const online = isUserOnline(otherUser.id);
                            
                            return (
                                <div 
                                    key={room.id} 
                                    className="room-item"
                                    onClick={() => setSelectedChat(otherUser)}
                                >
                                    <div className="room-avatar">
                                        {otherUser.username.charAt(0).toUpperCase()}
                                        <div className={`status-indicator ${online ? 'online' : 'offline'}`}></div>
                                    </div>
                                    <div className="room-info">
                                        <div className="room-header">
                                            <h4>{otherUser.username}</h4>
                                            {room.lastMessageAt && (
                                                <span className="last-message-time">
                                                    {new Date(room.lastMessageAt).toLocaleDateString()}
                                                </span>
                                            )}
                                        </div>
                                        <p className="room-status">
                                            {online ? 'Online' : 'Offline'}
                                        </p>
                                    </div>
                                </div>
                            );
                        })
                    )}
                </div>
            </div>

            {selectedChat && (
                <ChatModal
                    userId={selectedChat.id}
                    username={selectedChat.username}
                    onClose={() => setSelectedChat(null)}
                />
            )}
        </>
    );
}
