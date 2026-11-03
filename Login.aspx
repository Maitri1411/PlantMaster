<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs"
    Inherits="PlantMaster.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Login</title>

    <meta name="viewport"
          content="width=device-width, initial-scale=1.0" />

    <style>
        * {
            box-sizing: border-box;
        }

        body {
            margin: 0;
            padding: 20px;
            min-height: 100vh;
            font-family: Arial, Helvetica, sans-serif;
            background-color: #F4F7FB;

            display: flex;
            justify-content: center;
            align-items: center;
        }

        .login-card {
            width: 380px;
            max-width: 100%;
            padding: 32px;

            background-color: #EEF3FA;
            border: 1px solid #A8BCD9;
            border-radius: 18px;

            box-shadow: 0 10px 25px rgba(30, 55, 90, 0.15);
        }

        .title {
            text-align: center;
            color: #17365D;
            font-size: 24px;
            margin: 0 0 28px;
        }

        .label {
            display: block;
            margin-bottom: 7px;
            color: #17365D;
            font-size: 15px;
            font-weight: bold;
        }

        .input-box {
            display: block;
            width: 100%;
            height: 45px;
            padding: 10px 13px;
            margin-bottom: 20px;

            background-color: #FFFFFF;
            border: 1px solid #A8BCD9;
            border-radius: 8px;

            color: #17365D;
            font-size: 15px;
            outline: none;
        }

        .input-box:focus {
            border: 2px solid #6E8DBA;
        }

        .login-button {
            width: 100%;
            height: 46px;

            background-color: #A8BCD9;
            color: #17365D;

            border: 2px solid #6E8DBA;
            border-radius: 8px;

            font-size: 16px;
            font-weight: bold;
            cursor: pointer;
        }

        .login-button:hover {
            background-color: #6E8DBA;
            color: #FFFFFF;
        }

        .message {
            display: block;
            margin-bottom: 15px;
            font-size: 14px;
            overflow-wrap: anywhere;
        }

        @media (max-width: 600px) {
            .login-card {
                width: 100%;
                max-width: 360px;
                padding: 28px 24px;
            }

            .title {
                font-size: 21px;
            }
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <div class="login-card">

            <h2 class="title">USER LOGIN</h2>

            <asp:Label
                ID="lblUserName"
                runat="server"
                Text="User Name"
                CssClass="label" />

            <asp:TextBox
                ID="txtUserName"
                runat="server"
                CssClass="input-box"
                placeholder="Enter User Name"
                autocomplete="username" />

            <asp:Label
                ID="lblUserPassword"
                runat="server"
                Text="User Password"
                CssClass="label" />

            <asp:TextBox
                ID="txtUserPassword"
                runat="server"
                TextMode="Password"
                CssClass="input-box"
                placeholder="Enter User Password"
                autocomplete="current-password" />

            <asp:Label
                ID="lblMessage"
                runat="server"
                CssClass="message"
                EnableViewState="false" />

            <asp:Button
                ID="btnLogin"
                runat="server"
                Text="LOGIN"
                CssClass="login-button"
                OnClick="btnLogin_Click" />

        </div>

    </form>
</body>
</html>