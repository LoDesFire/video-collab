import axios from "axios"
import {toast} from "react-toastify";

export const handleError = (error: any) => {
    if (axios.isAxiosError(error)) {
        const err = error.response;
        if (Array.isArray((err?.data))) {
            err?.data.forEach((element) => {
                toast.error(element)
            })
        } else if (err?.status == 401) {
            toast.warning("Войдите в аккаунт")
            window.history.pushState({}, "LoginPage", "/login");
        } else if (typeof err?.data.errors === 'object') {
            for (let e in err?.data.errors) {
                toast.warning(err.data.errors[e[0]]);
            }
        } else if (err) {
            toast.warning(err?.data)
        }
    }
}