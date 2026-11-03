
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="PlantLogin.aspx.cs"
    Inherits="PlantMaster.PlantLogin" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Plant Login</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <style>
        * {
            box-sizing: border-box;
        }

        body {
            margin: 0;
            padding: 20px;
            min-height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            font-family: Arial, Helvetica, sans-serif;
            background: #F4F7FB;
        }

        .login-card {
            width: 380px;
            max-width: 100%;
            padding: 30px;
            background: #EEF3FA;
            border: 1px solid #A8BCD9;
            border-radius: 16px;
            box-shadow: 0 8px 22px rgba(30, 55, 90, 0.15);
        }

        h2 {
            text-align: center;
            color: #17365D;
            margin: 0 0 25px;
        }

        .field-label {
            display: block;
            margin-bottom: 8px;
            color: #17365D;
            font-weight: bold;
        }

        .field {
            width: 100%;
            height: 44px;
            margin-bottom: 20px;
            padding: 8px 10px;
            border: 1px solid #A8BCD9;
            border-radius: 7px;
            background: #FFFFFF;
            color: #17365D;
            font-size: 15px;
        }

        .login-button {
            width: 100%;
            height: 46px;
            border: 0;
            border-radius: 7px;
            background: #17365D;
            color: #FFFFFF;
            font-size: 16px;
            font-weight: bold;
            cursor: pointer;
        }

        .message {
            display: block;
            margin-top: 15px;
            text-align: center;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <div class="login-card">

            <h2>Plant Login</h2>

            <asp:Label
                ID="lblPlant"
                runat="server"
                Text="Select Plant"
                CssClass="field-label" />

            <asp:DropDownList
                ID="ddlPlant"
                runat="server"
                CssClass="field">
            </asp:DropDownList>

            <asp:Label
                ID="lblPassword"
                runat="server"
                Text="Plant Password"
                CssClass="field-label" />

            <asp:TextBox
                ID="txtPlantPassword"
                runat="server"
                TextMode="Password"
                CssClass="field"
                MaxLength="20" />

            <asp:Button
                ID="btnLogin"
                runat="server"
                Text="Login"
                CssClass="login-button"
                OnClick="btnLogin_Click" />

            <asp:Label
                ID="lblMessage"
                runat="server"
                CssClass="message" />

        </div>
    </form>
</body>
</html>