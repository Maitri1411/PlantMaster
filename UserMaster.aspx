
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="UserMaster.aspx.cs"
    Inherits="PlantMaster.UserMaster" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Details</title>

    <meta name="viewport"
          content="width=device-width, initial-scale=1.0" />

    <style>
        * {
            box-sizing: border-box;
        }

        body {
            margin: 0;
            padding: 15px;
            min-height: 100vh;
            font-family: Arial, Helvetica, sans-serif;
            background-color: #F4F7FB;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .user-card {
            width: 700px;
            max-width: 100%;
            padding: 15px 20px;
            background-color: #EEF3FA;
            border: 1px solid #A8BCD9;
            border-radius: 15px;
            box-shadow: 0px 6px 18px rgba(30, 55, 90, 0.12);
        }

        .title {
            text-align: center;
            color: #17365D;
            font-size: 20px;
            margin: 0 0 15px 0;
        }

        .user-details-box {
            padding: 12px;
            margin-bottom: 15px;
            background-color: #FFFFFF;
            border: 1px solid #A8BCD9;
            border-radius: 10px;
        }

        .details-row {
            display: grid;
            grid-template-columns: 1fr;
            gap: 12px;
            align-items: start;
        }

        .field {
            min-width: 0;
        }

        .label {
            display: block;
            font-size: 14px;
            font-weight: bold;
            color: #17365D;
            margin-bottom: 6px;
        }

        .input-box {
            width: 100%;
            height: 40px;
            padding: 8px 12px;
            font-size: 14px;
            color: #17365D;
            background-color: #FFFFFF;
            border: 1px solid #A8BCD9;
            border-radius: 7px;
            outline: none;
            display: block;
        }

        .input-box:focus {
            border: 2px solid #6E8DBA;
        }

        .active-field {
            padding: 5px 0;
            white-space: nowrap;
            color: #17365D;
            font-size: 14px;
        }

        .active-field input {
            accent-color: #6E8DBA;
        }

        .selection-box {
            padding: 12px;
            margin-bottom: 15px;
            background-color: #FFFFFF;
            border: 1px solid #A8BCD9;
            border-radius: 10px;
        }

        .section-title {
            color: #17365D;
            font-size: 15px;
            font-weight: bold;
            margin: 0 0 10px 0;
        }

        .selection-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 12px;
        }

        .selection-panel {
            min-width: 0;
            padding: 12px;
            background-color: #FFFFFF;
            border: 1px solid #A8BCD9;
            border-radius: 8px;
        }

        .panel-title {
            color: #17365D;
            font-size: 15px;
            font-weight: bold;
            margin: 0 0 10px 0;
            padding-bottom: 7px;
            border-bottom: 1px solid #A8BCD9;
        }

        .check-item {
            display: block;
            margin: 0 0 7px 0;
            color: #17365D;
            font-size: 13px;
        }

        .check-item input {
            margin-right: 6px;
            accent-color: #6E8DBA;
        }

        .save-area {
            text-align: center;
            margin-top: 15px;
        }

        .save-button {
            min-width: 150px;
            height: 40px;
            padding: 0 25px;
            background-color: #A8BCD9;
            color: #17365D;
            border: 2px solid #6E8DBA;
            border-radius: 8px;
            font-size: 15px;
            font-weight: bold;
            cursor: pointer;
        }

        .save-button:hover {
            background-color: #6E8DBA;
            color: #FFFFFF;
        }

        .message {
            display: block;
            margin-top: 10px;
            font-size: 13px;
            overflow-wrap: anywhere;
        }

        @media (max-width: 700px) {
            body {
                padding: 10px;
                align-items: flex-start;
            }

            .user-card {
                width: 100%;
                padding: 15px;
            }

            .selection-row {
                grid-template-columns: 1fr;
            }

            .input-box {
                height: 40px;
            }
        }

        @media (max-width: 420px) {
            .user-card {
                padding: 12px;
            }

            .user-details-box,
            .selection-box {
                padding: 10px;
            }

            .selection-panel {
                padding: 10px;
            }

            .title {
                font-size: 19px;
            }
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <div class="user-card">

            <h2 class="title">USER DETAILS</h2>

            <!-- USER INFORMATION -->
            <div class="user-details-box">

                <div class="details-row">

                    <div class="field">
                        <asp:Label
                            ID="lblFirstName"
                            runat="server"
                            Text="First Name"
                            CssClass="label" />

                        <asp:TextBox
                            ID="txtFirstName"
                            runat="server"
                            CssClass="input-box"
                            MaxLength="50"
                            placeholder="Enter First Name" />
                    </div>

                    <div class="field">
                        <asp:Label
                            ID="lblLastName"
                            runat="server"
                            Text="Last Name"
                            CssClass="label" />

                        <asp:TextBox
                            ID="txtLastName"
                            runat="server"
                            CssClass="input-box"
                            MaxLength="50"
                            placeholder="Enter Last Name" />
                    </div>

                    <div class="field">
                        <asp:Label
                            ID="lblUserName"
                            runat="server"
                            Text="User Name"
                            CssClass="label" />

                        <asp:TextBox
                            ID="txtUserName"
                            runat="server"
                            CssClass="input-box"
                            MaxLength="100"
                            placeholder="Enter User Name" />
                    </div>

                    <div class="field">
                        <asp:Label
                            ID="lblPassword"
                            runat="server"
                            Text="Password"
                            CssClass="label" />

                        <asp:TextBox
                            ID="txtPassword"
                            runat="server"
                            TextMode="Password"
                            CssClass="input-box"
                            MaxLength="100"
                            placeholder="Enter Password"
                            autocomplete="new-password" />
                    </div>

                    <!-- USER ROLE -->
                    <div class="field">
                        <asp:Label
                            ID="lblRole"
                            runat="server"
                            Text="User Role"
                            CssClass="label" />

                        <asp:DropDownList
                            ID="ddlRole"
                            runat="server"
                            CssClass="input-box">
                            <asp:ListItem
                                Text="Select Role"
                                Value="" />
                            <asp:ListItem
                                Text="Manager"
                                Value="Manager" />
                            <asp:ListItem
                                Text="Worker"
                                Value="Worker" />
                        </asp:DropDownList>
                    </div>

                    <div class="active-field">
                        <asp:CheckBox
                            ID="chkActive"
                            runat="server"
                            Text="Active"
                            Checked="true" />
                    </div>

                </div>
            </div>

            <!-- SCREEN AND LOCATION PERMISSIONS -->
            <div class="selection-box">

                <div class="section-title">
                    Screen Name &amp; Location
                </div>

                <div class="selection-row">

                    <!-- SCREEN NAME -->
                    <div class="selection-panel">

                        <h3 class="panel-title">Screen Name</h3>

                        <asp:CheckBox ID="chkAttendanceList"
                            runat="server" Text="AttendanceList"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkAttendanceReport"
                            runat="server" Text="AttendanceReport"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkInverterMaster"
                            runat="server" Text="InverterMaster"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkLogin"
                            runat="server" Text="Login"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkPlantMaster"
                            runat="server" Text="PlantMaster"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkSelect"
                            runat="server" Text="Select"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkUnitGeneration"
                            runat="server" Text="UnitGeneration"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkUnitGenerationList"
                            runat="server" Text="UnitGenerationList"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkWorkReport"
                            runat="server" Text="WorkReport"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkWorkReportList"
                            runat="server" Text="WorkReportList"
                            CssClass="check-item" />

                    </div>

                    <!-- LOCATION -->
                    <div class="selection-panel">

                        <h3 class="panel-title">Location</h3>

                        <asp:CheckBox ID="chkLocation1"
                            runat="server" Text="Location 1"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkLocation2"
                            runat="server" Text="Location 2"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkLocation3"
                            runat="server" Text="Location 3"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkLocation4"
                            runat="server" Text="Location 4"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkLocation5"
                            runat="server" Text="Location 5"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkLocation6"
                            runat="server" Text="Location 6"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkLocation7"
                            runat="server" Text="Location 7"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkLocation8"
                            runat="server" Text="Location 8"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkLocation9"
                            runat="server" Text="Location 9"
                            CssClass="check-item" />

                        <asp:CheckBox ID="chkLocation10"
                            runat="server" Text="Location 10"
                            CssClass="check-item" />

                    </div>
                </div>
            </div>

            <!-- SAVE -->
            <div class="save-area">

                <asp:Button
                    ID="btnSave"
                    runat="server"
                    Text="SAVE"
                    CssClass="save-button"
                    OnClick="btnSave_Click" />

                <asp:Label
                    ID="lblMessage"
                    runat="server"
                    CssClass="message" />

            </div>

        </div>

    </form>
</body>
</html>
