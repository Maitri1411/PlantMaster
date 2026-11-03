<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="UserMasterList.aspx.cs"
    Inherits="PlantMaster.UserMasterList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>User Master List</title>

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
            color: #17365D;
        }

        /* MAIN CARD */
        .user-card {
            width: 100%;
            max-width: 1100px;
            margin: 20px auto;
            padding: 15px 20px;
            background-color: #EEF3FA;
            border: 1px solid #A8BCD9;
            border-radius: 15px;
            box-shadow: 0px 6px 18px rgba(30, 55, 90, 0.12);
        }

        /* PAGE TITLE */
        .title {
            text-align: center;
            color: #17365D;
            font-size: 20px;
            margin: 0 0 18px 0;
        }

        /* HEADER */
        .header {
            display: flex;
            justify-content: flex-start;
            align-items: center;
            gap: 8px;
            flex-wrap: wrap;
            margin-bottom: 15px;
        }

        /* SEARCH AREA */
        .search-area {
            display: flex;
            align-items: center;
            gap: 8px;
            flex-wrap: wrap;
            min-width: 0;
        }

        /* SEARCH BOX */
        .search-box {
            width: 220px;
            max-width: 100%;
            height: 40px;
            padding: 8px 12px;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 14px;
            color: #17365D;
            background-color: #FFFFFF;
            border: 1px solid #A8BCD9;
            border-radius: 7px;
            outline: none;
        }

        .search-box:focus {
            border: 2px solid #6E8DBA;
        }

        /* BUTTONS */
        .btn {
            min-height: 40px;
            padding: 0 16px;
            background-color: #A8BCD9;
            color: #17365D;
            border: 2px solid #6E8DBA;
            border-radius: 8px;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 14px;
            font-weight: bold;
            cursor: pointer;
            white-space: nowrap;
        }

        .btn:hover {
            background-color: #6E8DBA;
            color: #FFFFFF;
        }

        /* TABLE BOX */
        .table-box {
            width: 100%;
            padding: 10px;
            background-color: #FFFFFF;
            border: 1px solid #A8BCD9;
            border-radius: 10px;
        }

        .grid-container {
            width: 100%;
            overflow-x: auto;
        }

        /* TABLE */
        .grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 14px;
            color: #17365D;
        }

        .grid th {
            background-color: #A8BCD9;
            color: #17365D;
            border: 1px solid #A8BCD9;
            padding: 11px 10px;
            text-align: left;
            font-size: 14px;
            font-weight: bold;
            white-space: nowrap;
        }

        .grid td {
            background-color: #FFFFFF;
            color: #17365D;
            padding: 10px;
            border: 1px solid #D5DFEC;
            vertical-align: middle;
        }

        .grid tr:nth-child(even) td {
            background-color: #F4F7FB;
        }

        .grid tr:hover td {
            background-color: #E8EFF8;
        }

        /* ACTIVE / INACTIVE */
        .status-active {
            color: #17365D;
            font-weight: bold;
        }

        .status-inactive {
            color: #17365D;
            font-weight: bold;
        }

        /* MESSAGE */
        .message {
            display: block;
            margin: 0 0 12px 0;
            color: #17365D;
            font-size: 13px;
            overflow-wrap: anywhere;
        }

        /* RESPONSIVE */
        @media (max-width: 700px) {

            body {
                padding: 10px;
            }

            .user-card {
                margin: 5px auto;
                padding: 15px;
            }

            .header {
                justify-content: flex-start;
            }

            .search-area {
                width: 100%;
                gap: 8px;
            }

            .search-box {
                flex: 1 1 150px;
                width: auto;
                min-width: 0;
            }

            .btn {
                padding: 0 12px;
            }

            .table-box {
                padding: 6px;
            }
        }

        @media (max-width: 420px) {

            .user-card {
                padding: 12px;
            }

            .title {
                font-size: 19px;
            }

            .search-area {
                gap: 6px;
            }

            .search-box {
                flex: 1 1 100%;
                width: 100%;
            }

            .btn {
                min-height: 38px;
                padding: 0 10px;
            }
        }
    </style>

</head>

<body>

    <form id="form1" runat="server">

        <div class="user-card">

            <!-- PAGE TITLE -->
            <h2 class="title">USER MASTER LIST</h2>

            <!-- SEARCH AND ADD BUTTONS -->
            <div class="header">

                <div class="search-area">

                    <asp:TextBox
                        ID="txtSearch"
                        runat="server"
                        CssClass="search-box"
                        placeholder="Search user..." />

                    <asp:Button
                        ID="btnSearch"
                        runat="server"
                        Text="Search"
                        CssClass="btn"
                        OnClick="btnSearch_Click" />

                    <asp:Button
                        ID="btnAddNew"
                        runat="server"
                        Text=" Add"
                        CssClass="btn"
                        OnClick="btnAddNew_Click" />

                </div>

            </div>

            <!-- MESSAGE -->
            <asp:Label
                ID="lblMessage"
                runat="server"
                CssClass="message" />

            <!-- USER TABLE -->
            <div class="table-box">

                <div class="grid-container">

                    <asp:GridView
                        ID="gvUsers"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="grid"
                        GridLines="None"
                        EmptyDataText="No users found."
                        UseAccessibleHeader="true">

                        <Columns>

                            <asp:TemplateField HeaderText="No.">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField
                                DataField="UserName"
                                HeaderText="User Name" />

                            <asp:BoundField
                                DataField="FirstName"
                                HeaderText="First Name" />

                            <asp:BoundField
                                DataField="LastName"
                                HeaderText="Last Name" />

                            <asp:TemplateField HeaderText="Status">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblStatus"
                                        runat="server"
                                        Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>'
                                        CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "status-active" : "status-inactive" %>' />

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                    </asp:GridView>

                </div>

            </div>

        </div>

    </form>

</body>
</html>