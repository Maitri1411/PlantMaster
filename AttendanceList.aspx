
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AttendanceList.aspx.cs"
    Inherits="PlantMaster.AttendanceList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Attendance List</title>
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
            background: #F4F7FB;
            color: #17365D;
        }

        form {
            width: 100%;
        }

        .content {
            width: 100%;
            padding: 8px 12px;
        }

        .list-card {
            width: 100%;
            max-width: 1000px;
            margin: 0 auto;
            padding: 15px;
            background: #EEF3FA;
            border: 1px solid #A8BCD9;
            border-radius: 12px;
            box-shadow: 0 6px 15px rgba(30, 55, 90, .10);
        }

        .list-heading {
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 10px;
            padding-bottom: 12px;
            margin-bottom: 20px;
            border-bottom: 1px solid #A8BCD9;
        }

        .list-title {
            margin: 0;
            font-size: 21px;
            color: #17365D;
            font-weight: bold;
        }

        .search-row {
            display: flex;
            align-items: center;
            flex-wrap: wrap;
            gap: 10px;
            margin-bottom: 18px;
        }

        .search-box {
            flex: 1;
            min-width: 200px;
            height: 38px;
            padding: 8px 10px;
            border: 1px solid #A8BCD9;
            border-radius: 7px;
            background: #FFFFFF;
            color: #17365D;
            font-size: 14px;
            font-family: Arial, Helvetica, sans-serif;
        }

        .search-box:focus {
            outline: none;
            border: 2px solid #6E8DBA;
        }

        /* Same styling for Report, Search, New and Edit buttons */
        .action-button,
        .edit-button {
            min-height: 32px;
            padding: 6px 12px;
            background: #A8BCD9;
            color: #17365D;
            border: 1px solid #6E8DBA;
            border-radius: 6px;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 13px;
            font-weight: bold;
            cursor: pointer;
            white-space: nowrap;
            transition: 0.3s ease;
        }

        .action-button:hover,
        .edit-button:hover {
            background: #6E8DBA;
            color: #FFFFFF;
        }

        .action-button:focus,
        .edit-button:focus {
            outline: none;
            box-shadow: 0 0 0 2px rgba(110, 141, 186, 0.25);
        }

        .table-wrap {
            width: 100%;
            overflow-x: auto;
            border: 1px solid #A8BCD9;
            border-radius: 7px;
            background: #FFFFFF;
        }

        .attendance-table {
            width: 100%;
            min-width: 850px;
            border-collapse: collapse;
            background: #FFFFFF;
        }

        .attendance-table th {
            padding: 12px 10px;
            text-align: left;
            background: #DCE7F5;
            color: #17365D;
            border: 1px solid #A8BCD9;
            font-size: 14px;
            font-weight: bold;
            white-space: nowrap;
        }

        .attendance-table td {
            padding: 10px;
            border: 1px solid #D2DDEC;
            color: #17365D;
            font-size: 14px;
            vertical-align: middle;
            overflow-wrap: anywhere;
        }

        .attendance-table tr:nth-child(even) {
            background: #F4F7FB;
        }

        .attendance-table tr:hover td {
            background: #E5EDF8;
        }

        .photo-link {
            color: #17365D;
            font-size: 14px;
            text-decoration: underline;
            font-weight: normal;
        }

        .photo-link:hover {
            color: #6E8DBA;
        }

        .empty-message {
            padding: 15px;
            color: #17365D;
            text-align: center;
            font-size: 14px;
        }

        @media (max-width: 600px) {
            body {
                padding: 8px;
            }

            .content {
                padding: 8px 2px;
            }

            .list-card {
                width: 100%;
                padding: 12px;
            }

            .list-heading {
                margin-bottom: 15px;
            }

            .search-row {
                align-items: stretch;
            }

            .search-box {
                min-width: 100%;
                width: 100%;
            }

            .action-button {
                flex: 1;
            }

            .attendance-table {
                min-width: 850px;
            }
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <div class="content">

            <div class="list-card">

                <div class="list-heading">
                    <h2 class="list-title">Attendance List</h2>
                </div>

                <div class="search-row">

                    <asp:TextBox
                        ID="txtSearch"
                        runat="server"
                        CssClass="search-box"
                        placeholder="Search by date or name" />

                    <asp:Button
   ID="btnsearch"
   runat="server"
   Text="Search"
   CssClass="action-button"
   OnClick="btnSearch_Click" />

                    <asp:Button
                        ID="btnReport"
                        runat="server"
                        Text="Report"
                        CssClass="action-button"
                        OnClick="btnReport_Click" />

                     
                    
                    
            
                    <asp:Button
                        ID="btnAdd"
                        runat="server"
                        Text="New"
                        CssClass="action-button"
                        OnClick="btnAdd_Click" />

                </div>

                <div class="table-wrap">

                    <asp:GridView
                        ID="gvAttendance"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="attendance-table"
                        GridLines="None"
                        DataKeyNames="AttendanceID"
                        ShowHeaderWhenEmpty="True"
                        EmptyDataText="No attendance records found."
                        OnRowCommand="gvAttendance_RowCommand">

                        <Columns>

                            <asp:BoundField
                                DataField="AttendanceID"
                                HeaderText="No." />

                            <asp:BoundField
                                DataField="AttendanceDate"
                                HeaderText="Date"
                                DataFormatString="{0:dd-MM-yyyy}" />

                            <asp:BoundField
                                DataField="PersonName"
                                HeaderText="Name" />

                            <asp:TemplateField HeaderText="In Photo">
                                <ItemTemplate>

                                    <asp:HyperLink
                                        ID="lnkInPhoto"
                                        runat="server"
                                        Text="View Photo"
                                        CssClass="photo-link"
                                        NavigateUrl='<%# GetPhotoUrl(Eval("InPhotoPath")) %>'
                                        Target="_blank"
                                        Visible='<%# HasPhoto(Eval("InPhotoPath")) %>' />

                                    <asp:Label
                                        ID="lblNoInPhoto"
                                        runat="server"
                                        Text="No Photo"
                                        Visible='<%# !HasPhoto(Eval("InPhotoPath")) %>' />

                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField
                                DataField="InTime"
                                HeaderText="In Time"
                                DataFormatString="{0:hh\:mm}"
                                NullDisplayText="—"
                                HtmlEncode="False" />

                            <asp:TemplateField HeaderText="Out Photo">
                                <ItemTemplate>

                                    <asp:HyperLink
                                        ID="lnkOutPhoto"
                                        runat="server"
                                        Text="View Photo"
                                        CssClass="photo-link"
                                        NavigateUrl='<%# GetPhotoUrl(Eval("OutPhotoPath")) %>'
                                        Target="_blank"
                                        Visible='<%# HasPhoto(Eval("OutPhotoPath")) %>' />

                                    <asp:Label
                                        ID="lblNoOutPhoto"
                                        runat="server"
                                        Text="No Photo"
                                        Visible='<%# !HasPhoto(Eval("OutPhotoPath")) %>' />

                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField
                                DataField="OutTime"
                                HeaderText="Out Time"
                                DataFormatString="{0:hh\:mm}"
                                NullDisplayText="—"
                                HtmlEncode="False" />

                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>

                                    <asp:Button
                                        ID="btnEdit"
                                        runat="server"
                                        Text="Edit"
                                        CssClass="edit-button"
                                        CommandName="EditAttendance"
                                        CommandArgument='<%# Eval("AttendanceID") %>'
                                        CausesValidation="false" />

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
