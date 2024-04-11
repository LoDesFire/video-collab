import axios from "axios"
import {handleError} from "../Helper/ErrorHandler";
import {UserProfileToken} from "../Models/User";

// const api = "/api/"

export const loginAPI = async (username: string, password: string) => {
    try {
        return await axios.post<UserProfileToken>("login", {
            username: username,
            password: password,
        })
    } catch (error) {
        handleError(error)
    }
}

export const registerAPI = async (username: string, password: string) => {
    try {
        return await axios.post<UserProfileToken>("register", {
            username: username,
            password: password,
        })
    } catch (error) {
        handleError(error)
    }
}
