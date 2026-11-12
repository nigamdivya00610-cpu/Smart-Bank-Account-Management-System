<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="SmartBank.Default"
    Async="true" %>

<!DOCTYPE html>

<html>
<head runat="server">

    <title>Smart Bank Account Management System</title>

    <style>

    body {
        margin: 0;
        font-family: Arial, sans-serif;
        background: #eef3f8;
    }

    .container {
        width: 700px;
        margin: 40px auto;
        background: white;
        padding: 30px;
        border-radius: 15px;
        box-shadow: 0 5px 20px rgba(0,0,0,0.12);
    }

    h1 {
        text-align: center;
        color: #123b63;
        margin-bottom: 10px;
    }

    h3 {
        color: #155a91;
        margin-top: 25px;
    }

    hr {
        border: 0;
        border-top: 1px solid #d5dce3;
        margin: 20px 0;
    }

    .textbox {
        width: 280px;
        padding: 10px;
        border: 1px solid #b9c5d0;
        border-radius: 6px;
        font-size: 14px;
        margin-top: 5px;
    }

    .textbox:focus {
        outline: none;
        border-color: #1769aa;
    }

    .button {
        padding: 10px 18px;
        margin: 6px 4px 6px 0;
        border: none;
        border-radius: 6px;
        background: #155a91;
        color: white;
        font-size: 14px;
        cursor: pointer;
    }

    .button:hover {
        background: #0d426b;
    }

    .message {
        margin-top: 25px;
        padding: 20px;
        background: #f5f8fb;
        border-left: 5px solid #155a91;
        border-radius: 8px;
        line-height: 1.6;
        min-height: 40px;
    }

    #GridView1 {
        width: 100%;
        margin-top: 15px;
        border-collapse: collapse;
    }

    #GridView1 th {
        background: #155a91;
        color: white;
        padding: 10px;
        text-align: left;
    }

    #GridView1 td {
        padding: 9px;
        border-bottom: 1px solid #ddd;
    }

</style>

</head>

<body>

<form id="form1" runat="server">

<div class="container">

    <h1>Smart Bank Account Management System</h1>

    <hr />

    <h3>Create Account</h3>

    Account Number:

    <br />

    <asp:TextBox
        ID="txtAccountNumber"
        runat="server"
        CssClass="textbox">
    </asp:TextBox>

    <br /><br />

    Account Holder Name:

    <br />

    <asp:TextBox
        ID="txtAccountHolder"
        runat="server"
        CssClass="textbox">
    </asp:TextBox>

    <br /><br />

    <asp:Button
        ID="btnCreate"
        runat="server"
        Text="Create Account"
        CssClass="button"
        OnClick="btnCreate_Click" />

    <hr />

    <h3>Bank Operations</h3>

    Amount:

    <br />

    <asp:TextBox
        ID="txtAmount"
        runat="server"
        CssClass="textbox">
    </asp:TextBox>

    <br /><br />

    <asp:Button
        ID="btnDeposit"
        runat="server"
        Text="Deposit"
        CssClass="button"
        OnClick="btnDeposit_Click" />

    <asp:Button
        ID="btnWithdraw"
        runat="server"
        Text="Withdraw"
        CssClass="button"
        OnClick="btnWithdraw_Click" />

    <br />

    <asp:Button
        ID="btnBalance"
        runat="server"
        Text="Display Balance"
        CssClass="button"
        OnClick="btnBalance_Click" />

    <asp:Button
        ID="btnTransactions"
        runat="server"
        Text="Transactions"
        CssClass="button"
        OnClick="btnTransactions_Click" />

    <br />

    <asp:Button
        ID="btnSaving"
        runat="server"
        Text="Saving Suggestions"
        CssClass="button"
        OnClick="btnSaving_Click" />

    <div class="message">

        <asp:Label
            ID="lblMessage"
            runat="server">
        </asp:Label>

        <br /><br />

        <asp:GridView
            ID="GridView1"
            runat="server"
            AutoGenerateColumns="true">
        </asp:GridView>

    </div>

</div>

</form>

</body>
</html>