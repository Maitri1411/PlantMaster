
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="InverterMaster.aspx.cs"
    Inherits="PlantMaster.InverterMaster" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Inverter Master</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

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
            color: #17365D;
        }

        form {
            width: 100%;
        }

        .top-bar {
            padding: 8px 12px;
        }

        .search-box {
            width: 320px;
            max-width: 100%;
            height: 38px;
            padding: 8px 12px;
            border: 1px solid #A8BCD9;
            border-radius: 8px;
            font-size: 14px;
            font-family: Arial, Helvetica, sans-serif;
            background: #FFFFFF;
            color: #17365D;
        }

        .content {
            padding: 8px 12px;
        }

        .heading-row {
            display: flex;
            align-items: center;
            flex-wrap: wrap;
            gap: 8px;
            padding: 0 5px 10px;
            border-bottom: 1px solid #A8BCD9;
            margin-bottom: 12px;
        }

        .heading {
            font-size: 23px;
            margin: 5px 0;
            color: #17365D;
        }

        .action-button {
            background-color: #A8BCD9;
            color: #17365D;
            border: 1px solid #6E8DBA;
            border-radius: 8px;
            padding: 8px 16px;
            font-size: 14px;
            font-weight: bold;
            cursor: pointer;
        }

        .action-button:hover,
        .edit-button:hover,
        .back-button:hover {
            background-color: #6E8DBA;
            color: #FFFFFF;
        }

        .table-wrapper {
            width: 100%;
            overflow-x: auto;
        }

        .grid {
            width: 100%;
            max-width: 850px;
            border-collapse: collapse;
            table-layout: fixed;
            background: #FFFFFF;
        }

        .grid th {
            background: #A8BCD9;
            color: #17365D;
            padding: 7px;
            text-align: left;
            font-size: 14px;
            border: 1px solid #A8BCD9;
            overflow-wrap: anywhere;
        }

        .grid td {
            padding: 6px 7px;
            font-size: 13px;
            border: 1px solid #D5DFED;
            color: #17365D;
            overflow-wrap: anywhere;
            vertical-align: middle;
        }

        /* Plant ID */
        .grid th:nth-child(1),
        .grid td:nth-child(1) {
            width: 75px;
        }

        /* Inverter ID */
        .grid th:nth-child(2),
        .grid td:nth-child(2) {
            width: 100px;
        }

        /* Inverter Name */
        .grid th:nth-child(3),
        .grid td:nth-child(3) {
            width: 170px;
        }

        /* Active Status */
        .grid th:nth-child(4),
        .grid td:nth-child(4) {
            width: 100px;
            text-align: center;
        }

        /* Edit button */
        .grid th:nth-child(5),
        .grid td:nth-child(5) {
            width: 65px;
            text-align: center;
        }

        .edit-button {
            background: #A8BCD9;
            color: #17365D;
            border: 1px solid #6E8DBA;
            border-radius: 6px;
            padding: 5px 8px;
            font-size: 12px;
            cursor: pointer;
        }

        .detail-card {
            width: 100%;
            max-width: 650px;
            padding: 15px;
            background: #EEF3FA;
            border: 1px solid #A8BCD9;
            border-radius: 12px;
            box-shadow: 0 6px 15px rgba(30,55,90,.10);
        }

        .detail-heading {
            display: flex;
            align-items: center;
            flex-wrap: wrap;
            gap: 8px;
            padding-bottom: 12px;
            border-bottom: 1px solid #A8BCD9;
            margin-bottom: 25px;
        }

        .back-button {
            background: #A8BCD9;
            color: #17365D;
            border: 1px solid #6E8DBA;
            border-radius: 8px;
            width: 40px;
            height: 36px;
            font-size: 22px;
            cursor: pointer;
        }

        .detail-title {
            font-size: 21px;
            margin: 0;
            color: #17365D;
        }

        .detail-row {
            display: grid;
            grid-template-columns: 110px minmax(0, 280px);
            align-items: center;
            gap: 12px;
            margin: 15px 10px;
        }

        .detail-label {
            text-align: right;
            font-size: 14px;
            color: #17365D;
        }

        .detail-input {
            width: 100%;
            height: 38px;
            padding: 8px 10px;
            border: 1px solid #A8BCD9;
            border-radius: 7px;
            font-size: 14px;
            font-family: Arial, Helvetica, sans-serif;
            color: #17365D;
            background: #FFFFFF;
        }

        .detail-input:focus {
            outline: none;
            border: 2px solid #6E8DBA;
        }

        .active-label {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 14px;
            color: #17365D;
        }

        @media (max-width: 600px) {
            body {
                padding: 8px;
            }

            .content {
                padding: 8px 2px;
            }

            .grid {
                min-width: 510px;
            }

            .grid th,
            .grid td {
                padding: 5px 3px;
                font-size: 11px;
            }

            .grid th:nth-child(1),
            .grid td:nth-child(1) {
                width: 60px;
            }

            .grid th:nth-child(2),
            .grid td:nth-child(2) {
                width: 75px;
            }

            .grid th:nth-child(3),
            .grid td:nth-child(3) {
                width: 120px;
            }

            .grid th:nth-child(4),
            .grid td:nth-child(4) {
                width: 75px;
            }

            .grid th:nth-child(5),
            .grid td:nth-child(5) {
                width: 55px;
            }

            .detail-row {
                grid-template-columns: 1fr;
                gap: 6px;
                margin: 14px 2px;
            }

            .detail-label {
                text-align: left;
            }

            .detail-card {
                padding: 10px;
            }
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <!-- Search -->
    <div class="top-bar">
        <asp:TextBox
            ID="txtSearch"
            runat="server"
            CssClass="search-box"
            placeholder="Search Inverter"
            AutoPostBack="true"
            OnTextChanged="txtSearch_TextChanged">
        </asp:TextBox>
    </div>

    <div class="content">

        <!-- Inverter List -->
        <asp:Panel ID="pnlList" runat="server">

            <div class="heading-row">
                <h2 class="heading">Inverter Master</h2>

                <asp:Button
                    ID="btnNew"
                    runat="server"
                    Text="+ New"
                    CssClass="action-button"
                    OnClick="btnNew_Click" />
            </div>

            <div class="table-wrapper">
                <asp:GridView
                    ID="gvInverterMaster"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="grid"
                    DataKeyNames="PlantID,InverterID"
                    EmptyDataText="No inverter records found."
                    OnRowCommand="gvInverterMaster_RowCommand">

                    <Columns>

                        <asp:BoundField
                            DataField="PlantID"
                            HeaderText="Plant ID" />

                        <asp:BoundField
                            DataField="InverterID"
                            HeaderText="Inverter ID" />

                        <asp:BoundField
                            DataField="InverterName"
                            HeaderText="Inverter Name" />

                        <asp:TemplateField HeaderText="Active Status">
                            <ItemTemplate>
                                <%# Convert.ToBoolean(Eval("IsActive"))
                                    ? "Active" : "Inactive" %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <asp:Button
                                    ID="btnEdit"
                                    runat="server"
                                    Text="Edit"
                                    CssClass="edit-button"
                                    CommandName="EditRecord"
                                    CommandArgument='<%# Container.DataItemIndex %>'
                                    CausesValidation="false" />
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </asp:GridView>
            </div>

        </asp:Panel>

        <!-- Add/Edit Form -->
        <asp:Panel
            ID="pnlDetails"
            runat="server"
            CssClass="detail-card"
            Visible="false">

            <div class="detail-heading">

                <asp:Button
                    ID="btnBack"
                    runat="server"
                    Text="‹"
                    CssClass="back-button"
                    CausesValidation="false"
                    OnClick="btnBack_Click" />

                <h2 class="detail-title">
                    <asp:Label
                        ID="lblFormTitle"
                        runat="server"
                        Text="Inverter Master">
                    </asp:Label>
                </h2>

                <asp:Button
                    ID="btnSave"
                    runat="server"
                    Text="Save"
                    CssClass="action-button"
                    OnClick="btnSave_Click" />

                <asp:Button
                    ID="btnNewDetail"
                    runat="server"
                    Text="+ New"
                    CssClass="action-button"
                    CausesValidation="false"
                    OnClick="btnNew_Click" />

            </div>

            <asp:HiddenField
                ID="hfOriginalPlantID"
                runat="server" />

            <asp:HiddenField
                ID="hfOriginalInverterID"
                runat="server" />

            <div class="detail-row">
                <asp:Label
                    ID="lblRecordID"
                    runat="server"
                    Text="Inverter ID:"
                    CssClass="detail-label" />

                <asp:Label
                    ID="lblRecordValue"
                    runat="server"
                    Text="0" />
            </div>

            <div class="detail-row">
                <asp:Label
                    ID="lblPlantID"
                    runat="server"
                    Text="Plant ID:"
                    AssociatedControlID="txtPlantID"
                    CssClass="detail-label" />

                <asp:TextBox
                    ID="txtPlantID"
                    runat="server"
                    CssClass="detail-input"
                    TextMode="Number"
                    placeholder="Enter Plant ID" />
            </div>

            <div class="detail-row">
                <asp:Label
                    ID="lblInverterID"
                    runat="server"
                    Text="Inverter ID:"
                    AssociatedControlID="txtInverterID"
                    CssClass="detail-label" />

                <asp:TextBox
                    ID="txtInverterID"
                    runat="server"
                    CssClass="detail-input"
                    MaxLength="25"
                    placeholder="Enter Inverter ID" />
            </div>

            <div class="detail-row">
                <asp:Label
                    ID="lblInverterName"
                    runat="server"
                    Text="Inverter Name:"
                    AssociatedControlID="txtInverterName"
                    CssClass="detail-label" />

                <asp:TextBox
                    ID="txtInverterName"
                    runat="server"
                    CssClass="detail-input"
                    MaxLength="25"
                    placeholder="Enter Inverter Name" />
            </div>

            <div class="detail-row">
                <span></span>
                <asp:CheckBox
                    ID="chkActive"
                    runat="server"
                    Text="Active"
                    Checked="true"
                    CssClass="active-label" />
            </div>

        </asp:Panel>

    </div>

</form>
</body>
</html>
