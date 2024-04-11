import React from "react";
import * as Yup from "yup";
import {useAuth} from "../../Context/useAuth";
import {useForm} from "react-hook-form";
import {yupResolver} from "@hookform/resolvers/yup";
import {Link} from "react-router-dom";

type Props = {};

type RegisterFormsInputs = {
    username: string;
    password: string;
    passwordRepeat: string;
};

const validation = Yup.object().shape({
    username: Yup.string().required("Введите ник!"),
    password: Yup.string().required("Введите пароль!").min(8, "Пароль должен быть не короче 8 символов"),
    passwordRepeat: Yup.string().required("Повторите пароль").when('password', (password, schema) => {
        return schema.test({
            test: (passwordRepeat) => password.toString() === passwordRepeat,
            message: 'Пароли не совпадают',
        });
    })
})

const RegisterPage = (props: Props) => {

    const {registerUser} = useAuth()
    const {
        register,
        handleSubmit,
        formState: {errors}
    } = useForm<RegisterFormsInputs>({resolver: yupResolver(validation)})

    const handleRegister = (form: RegisterFormsInputs) => {
        registerUser(form.username, form.password);
    }

    return (
        <section className="bg-white">
            <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto my-20 lg:py-20">
                <div
                    className="w-full bg-gray-50 rounded-lg shadow md:mb-20 sm:max-w-md xl:p-0">
                    <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
                        <h1 className="text-xl font-bold leading-tight tracking-tight text-gray-900 md:text-2xl">
                            Регистрация
                        </h1>
                        <form
                            className="space-y-4 md:space-y-6"
                            onSubmit={handleSubmit(handleRegister)}
                        >
                            <div>
                                <label
                                    htmlFor="username"
                                    className="block mb-2 text-sm font-medium text-gray-900"
                                />
                                <input
                                    type="text"
                                    id="username"
                                    className="bg-gray-50 border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5"
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
                                    className="block mb-2 text-sm font-medium text-gray-900"
                                >
                                    Пароль
                                </label>
                                <input
                                    type="password"
                                    id="password"
                                    placeholder="••••••••"
                                    className="bg-gray-50 border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5"
                                    {...register("password")}
                                />
                                {errors.password ? (
                                    <p className="text-black">{errors.password.message}</p>
                                ) : (
                                    ""
                                )}
                            </div>
                            <div>
                                <label
                                    htmlFor="passwordRepeat"
                                    className="block mb-2 text-sm font-medium text-gray-900"
                                >
                                    Повторите пароль
                                </label>
                                <input
                                    type="password"
                                    id="passwordRepeat"
                                    placeholder="••••••••"
                                    className="bg-gray-50 border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5"
                                    {...register("passwordRepeat")}
                                />
                                {errors.passwordRepeat ? (
                                    <p className="text-black">{errors.passwordRepeat.message}</p>
                                ) : (
                                    ""
                                )}
                            </div>
                            <button
                                type="submit"
                                className="w-full text-white bg-green-700 hover:bg-primary-700 focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center hover:font-bold"
                            >
                                Зарегистрироваться
                            </button>
                            <p className="text-sm font-light text-gray-500">
                                Есть аккаунт?{" "}
                                <Link
                                    to="/login"
                                    className="font-medium text-black hover:underline"
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
