function getAjax(url, success) {
    var xhr = window.XMLHttpRequest ? new XMLHttpRequest() : new ActiveXObject('Microsoft.XMLHTTP');
    xhr.open('GET', url);
    xhr.onreadystatechange = function () {
        if (xhr.readyState > 3 && xhr.status == 200) success(xhr.responseText);
    };
    xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    xhr.send();
    return xhr;
}

function postAjax(url, data, success) {
    var params = typeof data == 'string' ? data : Object.keys(data).map(
        function (k) { return encodeURIComponent(k) + '=' + encodeURIComponent(data[k]) }
    ).join('&');

    var xhr = window.XMLHttpRequest ? new XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
    xhr.open('POST', url);
    xhr.onreadystatechange = function () {
        if (xhr.readyState > 3 && xhr.status == 200) { success(xhr.responseText); }
    };
    xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest');
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xhr.send(params);
    return xhr;
}


function getCustomer() {
    var code = document.getElementById("code").value;
    getAjax('../../api/CustomerTA/FindCustomer?Code=' + code, function (data) {
        var obj = JSON.parse(data);
        document.getElementById('name').value = obj.Name;
        document.getElementById('fiscalId').value = obj.FiscalId;
        document.getElementById('address1').value = obj.AddressLine1;
        document.getElementById('address2').value = obj.AddressLine2;
        document.getElementById('postalcode').value = obj.PostalCode;
        document.getElementById('locality').value = obj.Locality;
        document.getElementById('payterm').value = obj.PaymentTerm;
        document.getElementById('subzone').value = obj.SubZone;
        document.getElementById('email').value = obj.Email;
        document.getElementById('new').checked = obj.GenerateNewCode;
    })
}


function saveCustomer() {
    var code = document.getElementById("code").value;

    var obj = {
        Name: document.getElementById('name').value,
        FiscalId : document.getElementById('fiscalId').value,
        AddressLine1 : document.getElementById('address1').value,
        AddressLine2 : document.getElementById('address2').value,
        PostalCode : document.getElementById('postalcode').value,
        Locality : document.getElementById('locality').value,
        PaymentTerm : document.getElementById('payterm').value,
        SubZone : document.getElementById('subzone').value,
        Email : document.getElementById('email').value,
        GenerateNewCode: document.getElementById('new').checked
    };

    postAjax('../../api/CustomerTA/GenerateCustomer', obj, function (data) {
        var obj = JSON.parse(data);
        document.getElementById("code").value = obj.Code;
    })
}