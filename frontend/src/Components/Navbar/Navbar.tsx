import React, {useState} from "react";
import {Link} from "react-router-dom";
import {useAuth} from "../../Context/useAuth";
import {AiOutlineClose, AiOutlineMenu} from 'react-icons/ai';

interface Props {
}

const Navbar = (props: Props) => {

    const {isLoggedIn, logout} = useAuth()

    const [nav, setNav] = useState(false);

    const handleNav = () => {
        setNav(!nav);
    };


    return (
        <div className='bg-white flex justify-between items-center h-24 max-w-[1240px] mx-auto px-4 text-black'>
            {/* Logo */}
            <div className="flex items-center space-x-10">
                <div className="w-full text-3xl font-bold text-back">
                    <Link to="/" className="hover:text-green-700 font-bold">
                        VideoCollab
                    </Link>
                </div>
                {
                    isLoggedIn() && (
                        <div className="hidden lg:flex items-center space-x-6 text-back" id="navbar-default">
                            <Link
                                className="hover:text-green-700"
                                to="/profile">
                                Профиль
                            </Link>
                            <button onClick={logout} className="hover:text-green-700">
                                Выйти
                            </button>
                        </div>
                    )
                }
            </div>

            {/* Desktop Navigation */}
            {
                !isLoggedIn() ? (
                    <div className="hidden lg:flex items-center space-x-6 text-back" id="navbar-default">
                        <Link
                            to="/login"
                            className="hover:text-green-700">
                            Войти
                        </Link>
                        <Link
                            to="/register"
                            className="px-8 py-3 font-bold rounded text-white bg-green-700 hover:opacity-95"
                        >
                            Зарегистрироваться
                        </Link>
                    </div>
                ) : (
                    <div className="hidden lg:flex items-center space-x-6 text-back" id="navbar-default">
                        <Link
                            className="hover:text-green-700"
                            to="/">
                            Присоедиться
                        </Link>
                        <h1> или </h1>
                        <Link
                            to="/"
                            className="px-8 py-3 font-bold rounded text-white bg-green-700 hover:opacity-95"
                        >
                            Создать
                        </Link>

                    </div>
                )
            }

            {/* Mobile Navigation Icon */}
            <div onClick={handleNav} className='block md:hidden'>
                {nav ? <AiOutlineClose size={20}/> : <AiOutlineMenu size={20}/>}
            </div>

            {/* Mobile Navigation Menu */}
            <ul
                className={
                    nav
                        ? 'fixed md:hidden left-0 top-0 w-[60%] h-full border-r border-r-gray-0 bg-white ease-in-out duration-500 space-y-10 p-10 text-[16px]'
                        : 'ease-in-out w-[60%] duration-500 fixed top-0 bottom-0 left-[-100%]'
                }
            >
                {/* Mobile Logo */}
                <li>
                    <div className="w-full text-3xl font-bold text-back">
                        <Link to="/" className="hover:text-green-700 font-bold">
                            VideoCollab
                        </Link>
                    </div>
                </li>
                <li>
                    {
                        isLoggedIn() ? (
                            <Link
                                className="hover:text-green-700"
                                to="/profile">
                                Профиль
                            </Link>
                        ) : (
                            <Link
                                className="hover:text-green-700"
                                to="/login">
                                Войти
                            </Link>
                        )
                    }
                </li>
                <li>
                    {
                        isLoggedIn() ? (
                            <Link
                                className="hover:text-green-700"
                                to="/profile">
                                Подключиться
                            </Link>
                        ) : (
                            <Link
                                className="hover:text-green-700"
                                to="/register">
                                Зарегистрироваться
                            </Link>
                        )
                    }
                </li>
                {
                    isLoggedIn() && (
                        <li>
                            <Link
                                className="hover:text-green-700"
                                to="/profile">
                                Создать
                            </Link>
                        </li>
                    )
                }
                {
                    isLoggedIn() && (
                        <li>
                            <Link
                                className="hover:text-green-700"
                                to="/profile">
                                Присоединиться
                            </Link>
                        </li>
                    )
                }
                {
                    isLoggedIn() && (
                        <li>
                            <button
                                className="hover:text-green-700"
                                onClick={logout}>
                                Выйти
                            </button>
                        </li>
                    )
                }

            </ul>
        </div>
    );
};

export default Navbar;
