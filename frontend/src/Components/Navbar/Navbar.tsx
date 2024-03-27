import React from "react";
import {Link} from "react-router-dom";
import logo from "./logo.png";

interface Props {
}

const Navbar = (props: Props) => {
    return (
        <nav className="relative container mx-auto p-6">
            <div className="flex items-center justify-between">
                <div className="flex items-center space-x-20">
                    <Link to="/">
                        <div className="hidden lg:flex items-center space-x-6 text-back">
                            <Link to="/" className="hover:text-darkBlue">
                                VideoCollab
                            </Link>
                        </div>
                    </Link>
                </div>
                <div className="hidden lg:flex items-center space-x-6 text-back">
                    <Link to="/login" className="hover:text-darkBlue">
                        Войти
                    </Link>
                    <Link
                        to="/register"
                        className="px-8 py-3 font-bold rounded text-white bg-lightGreen hover:opacity-70"
                    >
                        Зарегистрироваться
                    </Link>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;
