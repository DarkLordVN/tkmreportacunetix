function InBrowserVanBanDen() {
	elem = '#table-print';
	// elem chinh la id bao ngoai phan HTML muon in
	var mywindow = window.open('', 'my div', 'height= 700,width=1280');
	mywindow.document.write('<html><head>');
	mywindow.document.write('<link href="/Assets/css/all.css" rel="stylesheet" type="text/css" />');
	mywindow.document.write('<link href="/Assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />');
	mywindow.document.write('<link href="/Assets/css/mdb.min.css" rel="stylesheet" type="text/css" />');
	mywindow.document.write('<link href="/Assets/css/addons/datatables.min.css" rel="stylesheet" type="text/css" />');
	mywindow.document.write('<link href="/Assets/css/custom.css" rel="stylesheet" type="text/css" />');
	mywindow.document.write('<link href="/Assets/css/style.css" rel="stylesheet" type="text/css" />');
	mywindow.document.write('<script src="/Assets/js/jquery-1.9.1.min.js"></script>');
	mywindow.document.write('<style type="text/css" media="print">@page { size: landscape; }</style>');
	mywindow.document.write('</head><body>');
	var data = '<div>' + $(elem).html() + '</div>';
	mywindow.document.write(data);
	mywindow.document.write('<script>$(document).ready(function(){$(".pagination-ys").hide();$(".col-tacgia").css("width","130px");$(".col-trichyeu").css("width","220px");$(".col-dvnhan").css("width","130px");$(".dn-browser").css("display","none")})</script></body></html>');
	mywindow.document.close();
	mywindow.onload = function () {
		mywindow.focus();
		mywindow.print();
		mywindow.close();
	};
	
	return true;
	//$("input[type=number]").each(function () {this.replaceWith("<span>"'+this.val()+'"</span>");});
}
//In Van Ban Di
function InBrowserVanBanDi() {
    elem = '#table-print';
    // elem chinh la id bao ngoai phan HTML muon in
    var mywindow = window.open('', 'my div', 'height= 700,width=1200');
    mywindow.document.write('<html><head>');
    mywindow.document.write('<link href="/Assets/css/all.css" rel="stylesheet" type="text/css" />');
    mywindow.document.write('<link href="/Assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />');
    mywindow.document.write('<link href="/Assets/css/mdb.min.css" rel="stylesheet" type="text/css" />');
    mywindow.document.write('<link href="/Assets/css/addons/datatables.min.css" rel="stylesheet" type="text/css" />');
    mywindow.document.write('<link href="/Assets/css/custom.css" rel="stylesheet" type="text/css" />');
    mywindow.document.write('<link href="/Assets/css/style.css" rel="stylesheet" type="text/css" />');
    mywindow.document.write('<script src="/Assets/js/jquery-1.9.1.min.js"></script>');
    mywindow.document.write('<style type="text/css" media="print">@page { size: landscape; }</style>');
    mywindow.document.write('</head><body>');
    var data = '<div>' + $(elem).html() + '</div>';
    mywindow.document.write(data);
    mywindow.document.write('<script>$(document).ready(function(){$(".pagination-ys").hide();$(".col-sokyhieu").css("width","100px !important");$(".col-trichyeu").css("width","100px !important");$(".col-dvnhan").css("width","100px");$(".dn-browser").css("display","none")})</script></body></html>');
    mywindow.document.close();
    mywindow.onload = function () {
        mywindow.focus();
        mywindow.print();
        mywindow.close();
    };

    return true;
    //$("input[type=number]").each(function () {this.replaceWith("<span>"'+this.val()+'"</span>");});
}
function InBrowserNguoiDung() {
	elem = '#table-print';
	// elem chinh la id bao ngoai phan HTML muon in
	var mywindow = window.open('', 'my div', 'height= 700,width=1280');
	mywindow.document.write('<html><head>');
	mywindow.document.write('<link href="/Assets/css/all.css" rel="stylesheet" type="text/css" />');
	mywindow.document.write('<link href="/Assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />');
	mywindow.document.write('<link href="/Assets/css/mdb.min.css" rel="stylesheet" type="text/css" />');
	mywindow.document.write('<link href="/Assets/css/addons/datatables.min.css" rel="stylesheet" type="text/css" />');
	mywindow.document.write('<link href="/Assets/css/custom.css" rel="stylesheet" type="text/css" />');
	mywindow.document.write('<link href="/Assets/css/style.css" rel="stylesheet" type="text/css" />');
	mywindow.document.write('<script src="/Assets/js/jquery-1.9.1.min.js"></script>');
	mywindow.document.write('<style type="text/css" media="print">@page { size: landscape; }</style>');
	mywindow.document.write('</head><body>');
	var data = '<div>' + $(elem).html() + '</div>';
	mywindow.document.write(data);
	mywindow.document.write('<script>$(document).ready(function(){$(".pagination-ys").hide();$(".col-tacgia").css("width","130px");$(".col-trichyeu").css("width","220px");$(".col-dvnhan").css("width","130px");$(".dn-browser").css("display","none")})</script></body></html>');
	mywindow.document.close();
	mywindow.onload = function () {
		mywindow.focus();
		mywindow.print();
		mywindow.close();
	};

	return true;
	//$("input[type=number]").each(function () {this.replaceWith("<span>"'+this.val()+'"</span>");});
}