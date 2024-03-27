import axios from "axios"
import {handleError} from "../Helper/ErrorHandler";
import {UserProfileToken} from "../Models/User";

const api = "https://localhost:5555/"

export const loginAPI = async (username: string, password: string) => {
    try {
        return await axios.post<UserProfileToken>(api + "auth/login", {
            username: username,
            password: password,
        })
    } catch (error) {
        handleError(error)
    }
}

export const registerAPI = async (email: string, username: string, password: string) => {
    try {
        return await axios.post<UserProfileToken>(api + "auth/register", {
            email: email,
            username: username,
            password: password,
        })
    } catch (error) {
        handleError(error)
    }
}

