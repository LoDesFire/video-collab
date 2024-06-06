import axios from "axios"
import {handleError} from "../Helper/ErrorHandler";
import {UserProfileToken} from "../Models/UserDto";

export class AuthService {
    static async loginAPI(username: string, password: string) {
        try {
            return await axios.post<UserProfileToken>("login", {
                username: username,
                password: password,
            })
        } catch (error) {
            handleError(error)
        }
    }

    static async registerAPI(username: string, password: string) {
        try {
            return await axios.post<UserProfileToken>("register", {
                username: username,
                password: password,
            })
        } catch (error) {
            handleError(error)
        }
    }
}