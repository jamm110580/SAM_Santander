<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Inicio.aspx.cs" Inherits="Inicio" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <style>
        .hero-container {
            position: relative;
            background-image: url('img/Fondo_Oficina.jpg'); /* Cambia por la ruta de tu imagen */
            background-size: cover;
            background-position: center;
            height: 88vh;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.7);
        }

        .hero-title {
            font-size: 3em;
            font-weight: bold;
            background-color: rgba(0, 0, 0, 0.5); /* Suave overlay para mejorar lectura */
            padding: 20px;
            border-radius: 10px;
        }

        footer {
            width: 100%;
            background-color: rgba(0,0,0,0.6);
            color: white;
            text-align: center;
            padding: 10px 0;
            position: relative; /* Así se queda al final natural del contenido */
        }
    </style>

    <div class="hero-container">
        <div class="hero-title">Sistema de Conciliación Bancaria</div>
    </div>

    
</asp:Content>
