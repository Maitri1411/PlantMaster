   
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="PlantMaster.aspx.cs"
    Inherits="PlantMaster.PlantMaster" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Plant Master</title>
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

        .grid {
            width: 100%;
            max-width: 650px;
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
        }

        .grid th:first-child,
        .grid td:first-child {
            width: 75px;
        }

        .grid th:nth-child(2),
        .grid td:nth-child(2) {
            width: 170px;
        }

        .grid th:nth-child(3),
        .grid td:nth-child(3) {
            width: 100px;
            text-align: center;
        }

        .grid th:nth-child(4),
        .grid td:nth-child(4) {
            width: 65px;
            text-align: center;
        }

        
.edit-button {
    background: #A8BCD9;
    color: #17365D;
    border: 1px solid #6E8DBA;
    border-radius: 6px;
    padding: 4px 5px;
    font-size: 11px;
    width: 42px;
    height: 26px;
    cursor: pointer;
    white-space: nowrap;
}
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
            grid-template-columns: 110px minmax(0,280px);
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

            .grid th,
            .grid td {
                padding: 5px 3px;
                font-size: 11px;
            }

            .grid th:first-child,
            .grid td:first-child {
                width: 60px;
            }

            .grid th:nth-child(2),
            .grid td:nth-child(2) {
                width: 120px;
            }

            .grid th:nth-child(3),
            .grid td:nth-child(3) {
                width: 75px;
            }

            .grid th:nth-child(4),
            .grid td:nth-child(4) {
                width: 50px;
            }

            .detail-row {
                grid-template-columns: 1fr;
                gap: 6px;
                margin: 14px 2px;
            }

            .detail-label {
                text-align: left;
            }
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <div class="top-bar">
        <asp:TextBox
            ID="txtSearch"
            runat="server"
            CssClass="search-box"
            placeholder="Search Plant"
            AutoPostBack="true"
            OnTextChanged="txtSearch_TextChanged">
        </asp:TextBox>
    </div>

    <div class="content">

        <asp:Panel ID="pnlList" runat="server">

            <div class="heading-row">
                <h2 class="heading">Plant Master</h2>

                <asp:Button
                    ID="btnNew"
                    runat="server"
                    Text="+ New"
                    CssClass="action-button"
                    OnClick="btnNew_Click" />
            </div>

            <asp:GridView
                ID="gvPlantMaster"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="grid"
                DataKeyNames="PlantID"
                EmptyDataText="No plant records found."
                OnRowCommand="gvPlantMaster_RowCommand">

                <Columns>
                    <asp:BoundField
                        DataField="PlantID"
                        HeaderText="Plant ID" />

                    <asp:BoundField
                        DataField="PlantName"
                        HeaderText="Plant Name" />

                   
    
<asp:TemplateField HeaderText="Password">
    <ItemTemplate>
        <asp:Label
            ID="lblPassword"
            runat="server"
            Text='<%# Eval("Password") %>'>
        </asp:Label>
    </ItemTemplate>
</asp:TemplateField>


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
                                CommandArgument='<%# Container.DataItemIndex %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

        </asp:Panel>

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
                    OnClick="btnBack_Click" />

                <h2 class="detail-title">Plant Master</h2>

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
                    OnClick="btnNew_Click" />

            </div>

            <asp:HiddenField
                ID="hfOriginalPlantID"
                runat="server" />

            <div class="detail-row">
                <asp:Label
                    ID="lblRecordID"
                    runat="server"
                    Text="ID:"
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
                    CssClass="detail-label" />

                <asp:TextBox
                    ID="txtPlantID"
                    runat="server"
                    CssClass="detail-input" />
            </div>

            <div class="detail-row">
                <asp:Label
                    ID="lblPlantName"
                    runat="server"
                    Text="Plant Name:"
                    CssClass="detail-label" />

                <asp:TextBox
                    ID="txtPlantName"
                    runat="server"
                    CssClass="detail-input"
                    MaxLength="25"
                    placeholder="Enter Plant Name" />
            </div>

            <div class="detail-row">
                <asp:Label
                    ID="lblPassword"
                    runat="server"
                    Text="Password:"
                    CssClass="detail-label" />

                <asp:TextBox
                    ID="txtPassword"
                    runat="server"
                    CssClass="detail-input"
                    TextMode="Password"
                    MaxLength="20"
                    placeholder="Enter Password" />
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
