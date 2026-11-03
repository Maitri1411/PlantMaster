<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Select.aspx.cs"
    Inherits="PlantMaster.Select" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Select</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

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

        .select-card {
            width: 420px;
            max-width: 100%;
            padding: 35px 38px;
            background-color: #EEF3FA;
            border: 1px solid #A8BCD9;
            border-radius: 18px;
            box-shadow: 0 10px 25px rgba(30, 55, 90, 0.15);
        }

        .title {
            text-align: center;
            color: #17365D;
            font-size: 24px;
            margin: 0 0 25px;
        }

        .location-box {
            display: block;
            width: 100%;
            padding: 13px;
            margin-bottom: 22px;
            background-color: #FFFFFF;
            color: #17365D;
            border: 2px solid #A8BCD9;
            border-radius: 8px;
            text-align: center;
            font-size: 15px;
            font-weight: bold;
        }

        .menu-button {
            display: block;
            width: 100%;
            min-height: 50px;
            margin-bottom: 15px;
            background-color: #A8BCD9;
            color: #17365D;
            border: 2px solid #6E8DBA;
            border-radius: 8px;
            font-size: 16px;
            font-weight: bold;
            cursor: pointer;
        }

        .menu-button:hover {
            background-color: #6E8DBA;
            color: #FFFFFF;
        }

        .message {
            display: block;
            margin-bottom: 15px;
            color: #B00020;
            font-size: 13px;
            text-align: center;
            overflow-wrap: anywhere;
        }

        @media (max-width: 600px) {
            body {
                padding: 20px;
            }

            .select-card {
                width: 100%;
                max-width: 360px;
                padding: 30px 25px;
            }

            .title {
                font-size: 21px;
            }

            .menu-button {
                min-height: 46px;
            }
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <div class="select-card">

            <h2 class="title">SELECT</h2>

            <asp:Label
                ID="lblSelectedLocation"
                runat="server"
                CssClass="location-box"
                Visible="false">
            </asp:Label>

            <asp:Label
                ID="lblMessage"
                runat="server"
                CssClass="message">
            </asp:Label>

            <asp:Button
                ID="btnUnitGeneration"
                runat="server"
                Text="Unit Generation"
                CssClass="menu-button"
                Visible="false"
                OnClick="btnUnitGeneration_Click" />

            <asp:Button
                ID="btnWorkReport"
                runat="server"
                Text="Work Report"
                CssClass="menu-button"
                Visible="false"
                OnClick="btnWorkReport_Click" />

            <asp:Button
                ID="btnAttendanceReport"
                runat="server"
                Text="Attendance Report"
                CssClass="menu-button"
                Visible="false"
                OnClick="btnAttendanceReport_Click" />

        </div>

    </form>
</body>
</html>