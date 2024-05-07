import React from 'react';
import ReactDOM from 'react-dom/client';
import { RouterProvider } from "react-router-dom";
import './index.css';
import {router} from "./Routes/Routes";
import reportWebVitals from './reportWebVitals';
import axios from "axios";

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
);
axios.defaults.baseURL = "/api/"

reportWebVitals();
