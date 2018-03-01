

function SelectAll() {
    var tbl = document.getElementById('ctl00_ContentPlaceHolder1_dgvResult');
    var cbxTemp = null;
    var cbxAALL = tbl.rows[0].cells[0].getElementsByTagName('INPUT')[0];
    for (var i = 1; i < tbl.rows.length; i++) {
        cbxTemp = tbl.rows[i].cells[0].getElementsByTagName('INPUT')[0];
        cbxTemp.checked = cbxAALL.checked;
    }

}