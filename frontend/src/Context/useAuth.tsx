import {UserProfileId} from "../Models/UserDto";
import React, {createContext, useEffect, useState} from "react";
import {useNavigate} from "react-router-dom";
import {AuthService} from "../Services/AuthService";
import {toast} from "react-toastify";
import axios from "axios";

type UserContextType = {
    user: UserProfileId | null;
    token: string | null;
    loginUser: (username: string, password: string) => void;
    registerUser: (username: string, password: string) => void;
    logout: () => void;
    isLoggedIn: () => boolean;
};

type Props = { children: React.ReactNode };

const UserContext = createContext<UserContextType>({} as UserContextType);

export const UserProvider = ({children}: Props) => {
    const navigate = useNavigate();
    const [token, setToken] = useState<string | null>(null);
    const [user, setUser] = useState<UserProfileId | null>(null);
    const [isReady, setIsReady] = useState(false);

    useEffect(() => {
        const user = localStorage.getItem("user");
        const token = localStorage.getItem("token");
        if (user && token) {
            setUser(JSON.parse(user));
            setToken(token);
            axios.defaults.headers.common.Authorization = "Bearer " + token;
        }
        setIsReady(true);
    }, []);

    const loginUser = async (
        username: string,
        password: string
    ) => {
        await AuthService.loginAPI(username, password)
            .then((res) => {
                if (res) {
                    localStorage.setItem("token", res?.data.token)
                    const userObj: UserProfileId = {
                        id: res?.data.id,
                        username: res?.data.username,
                    }
                    localStorage.setItem("user", JSON.stringify(userObj));
                    setToken(res?.data.token)
                    axios.defaults.headers.common.Authorization = "Bearer " + res?.data.token;
                    navigate("/profile")
                    setUser(userObj!)
                }
            }).catch(() => toast.error("Ошибка сервера"));
    }


    const registerUser = async (
        username: string,
        password: string
    ) => {
        await AuthService.registerAPI(username, password)
            .then((res) => {
                if (res) {
                    localStorage.setItem("token", res?.data.token)
                    const userObj: UserProfileId = {
                        id: res?.data.id,
                        username: res?.data.username,
                    }
                    localStorage.setItem("user", JSON.stringify(userObj));
                    setToken(username)
                    axios.defaults.headers.common.Authorization = "Bearer " + res?.data.token;
                    setUser(userObj!)
                    navigate("/profile")
                }
            }).catch(() => toast.error("Ошибка сервера"));
    }

    const isLoggedIn = () => {
        return !!user;
    }

    const logout = () => {
        axios.defaults.headers.common.Authorization = null;
        localStorage.removeItem("token")
        localStorage.removeItem("user")
        setUser(null);
        setToken(null);
        navigate("/login")
    }

    return (
        <UserContext.Provider value={{user, token, loginUser, registerUser, logout, isLoggedIn}}>
            {isReady ? children : null}
        </UserContext.Provider>
    )
}

export const useAuth = () => React.useContext(UserContext);