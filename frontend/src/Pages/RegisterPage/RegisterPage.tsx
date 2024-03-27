import React from "react";
import HomePageComponent from "../../Components/HomePage/HomePageComponent";
import * as Yup from "yup";
import {useAuth} from "../../Context/useAuth";
import {useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";
import {Link} from "react-router-dom";
import {toast, ToastOptions} from "react-toastify";

type Props = {};

type RegisterFormsInputs = {
    email: string;
    username: string;
    password: string;
};

const validation = Yup.object().shape({
    username: Yup.string().required("Введите ник!"),
    password: Yup.string().required("Введите пароль!").min(8, "Пароль должен быть не короче 8 символов"),
    email: Yup.string().required("Введите Email!").email("Неверный формат почты!"),
})

const RegisterPage = (props: Props) => {

    const {registerUser} = useAuth()
    const {
        register,
        handleSubmit,
        formState: {errors}
    } = useForm<RegisterFormsInputs>({resolver: yupResolver(validation)})

    const handleRegister = (form: RegisterFormsInputs) => {
        toast.error("Hello", {
            autoClose: false,
            draggable: true,
            position: "bottom-right"
        });
        toast.warning("Goodbye");
        toast.success("YEEEES")
        registerUser(form.email, form.username, form.password);
    }

    return (
        <section className="bg-gray-50 dark:bg-gray-900">
            <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto md:h-screen lg:py-0">
                <div
                    className="w-full bg-white rounded-lg shadow dark:border md:mb-20 sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700">
                    <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
                        <h1 className="text-xl font-bold leading-tight tracking-tight text-gray-900 md:text-2xl dark:text-white">
                            Регистрация
                        </h1>
                        <form
                            className="space-y-4 md:space-y-6"
                            onSubmit={handleSubmit(handleRegister)}
                        >
                            <div>
                                <label
                                    htmlFor="email"
                                    className="block mb-2 text-sm font-medium text-gray-900 dark:text-white"
                                />
                                <input
                                    type="text"
                                    id="email"
                                    className="bg-gray-50 border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                                    placeholder="Email"
                                    {...register("email")}
                                />
                                {errors.email ? (
                                    <p className="text-black">{errors.email.message}</p>
                                ) : (
                                    ""
                                )}
                            </div>
                            <div>
                                <label
                                    htmlFor="username"
                                    className="block mb-2 text-sm font-medium text-gray-900 dark:text-white"
                                />
                                <input
                                    type="text"
                                    id="username"
                                    className="bg-gray-50 border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                                    placeholder="Username"
                                    {...register("username")}
                                />
                                {errors.username ? (
                                    <p className="text-black">{errors.username.message}</p>
                                ) : (
                                    ""
                                )}
                            </div>
                            <div>
                                <label
                                    htmlFor="password"
                                    className="block mb-2 text-sm font-medium text-gray-900 dark:text-white"
                                >
                                    Пароль
                                </label>
                                <input
                                    type="password"
                                    id="password"
                                    placeholder="••••••••"
                                    className="bg-gray-50 border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                                    {...register("password")}
                                />
                                {errors.password ? (
                                    <p className="text-black">{errors.password.message}</p>
                                ) : (
                                    ""
                                )}
                            </div>
                            <button
                                type="submit"
                                className="w-full text-white bg-lightGreen hover:bg-primary-700 focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center dark:bg-primary-600 dark:hover:bg-primary-700 dark:focus:ring-primary-800"
                            >
                                Зарегистрироваться
                            </button>
                            <p className="text-sm font-light text-gray-500 dark:text-gray-400">
                                Есть аккаунт?{" "}
                                <Link
                                    to="/login"
                                    className="font-medium text-primary-600 hover:underline dark:text-primary-500"
                                >
                                    Войдите
                                </Link>
                            </p>
                        </form>
                    </div>
                </div>
            </div>
        </section>
    );
};

export default RegisterPage;
