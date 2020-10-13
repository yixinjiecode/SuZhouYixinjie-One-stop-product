<%@ Control Language="C#" AutoEventWireup="true" CodeFile="YXJGridView.ascx.cs" Inherits="YXJGridView" %>
<%@ Register src="Pager_Background.ascx" tagname="Pager" tagprefix="uc1" %>
<script src="/js/yxjGridView.js" type="text/javascript"></script>
<div style="width: 100%; clear: both;" id="yxjgridview">
    <asp:gridview id="gv1" runat="server" onsorting="gv1_Sorting" onrowdatabound="gv1_RowDataBound" SkinID="gridview_Common"></asp:gridview>
</div>
<div id="Pager">
    <uc1:Pager ID="Pager1" runat="server" />
</div>
