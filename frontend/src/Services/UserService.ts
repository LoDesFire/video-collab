import axios from "axios";
import {UserProfileInfo} from "../Models/User";
import {handleError} from "../Helper/ErrorHandler";


export const userInfoGetAPI = async () => {
    try {
        return await axios.get<UserProfileInfo>("user/profile")
    } catch (error) {
        handleError(error)
    }
}

export const userPinMovieAPI = async (id: number) => {
    try {
        return await axios.put("user/pinnedMovies?movieId=" + id)
    } catch (error) {
        handleError(error)
    }
}

export const userUnpinMovieAPI = async (id: number) => {
    try {
        return await axios.delete("user/pinnedMovies", {params: {movieId: id}})
    } catch (error) {
        handleError(error)
    }
}