function notification(userloginid) {
	var tbao = $.connection.thongBaoHub;
	tbao.client.reloadNotification = function (lstnguoinhan, lstidmoinhat) {
		LoadDataNotification();
		//LoadDataNotificationRight();
		var currUserID = $('#currUserID').val();
		var increNoti = 0;
		var setStrMoiNhatCu = $('#lstidDaNhanMoiNhat').val();
		var setStrMoiNhat = "";
		var arrItemNguoiNhan = lstnguoinhan.split('[--]');
		var arrItemIDTB = lstidmoinhat.split(',');
		for (var i = 0; i < arrItemNguoiNhan.length; i++) {
			if (arrItemNguoiNhan[i] != '' && arrItemNguoiNhan[i].includes(',' + currUserID.toString() + ',')) {
				++increNoti;
				setStrMoiNhat += arrItemIDTB[i] + ',';
			}
		}
		$('#lstidDaNhanMoiNhat').val(setStrMoiNhatCu + setStrMoiNhat);
		var notification_count = $('#notification_count').text();
		var notification_count_new = $('#notification_count_new').text();
		if (notification_count == '') notification_count = 0;
		if (notification_count_new == '') notification_count_new = 0;
		notification_count = parseInt(notification_count) + increNoti;
		$('#notification_count').text(notification_count);
		$('#notification_count_new').text(parseInt(notification_count_new) + increNoti);
		$("#notification_count").fadeIn("slow");
	};
	$.connection.hub.start().done(function () {
		$('.addNotification').click(function () {
			tbao.server.uploadNotification(',' + $('#lstidThongBao').val() + ',');
		});
	});
}

//function notificationRight(userloginid) {
//    var tbao = $.connection.thongBaoHub;
//    tbao.client.reloadNotificationRight = function (thongtinnguoigui, noidung, listnguoinhanid) {
//        var currUserID = $('#currUserID').val();
//        if ($('#thongbaomoi').text().trim().toLowerCase() == "dữ liệu đang cập nhật..." && listnguoinhanid != '\""' && listnguoinhanid != '' && listnguoinhanid.includes(',' + currUserID.toString() + ',')) {
//            var ele = '<ul class="list-unstyled" id="reloadNotificationRight"><li class="media text-right pt-3"><a class="d-inline-block" href="/ThongBao?tab=1">Xem thêm <i class="fal fa-angle-double-right"></i></a></li></ul>';
//            $('#thongbaomoi').html(ele);
//        }
//        if (listnguoinhanid != '\""' && listnguoinhanid != '' && listnguoinhanid.includes(',' + currUserID.toString() + ',')) {
//            if ($('ul#reloadNotificationRight > li.rm').length > 4) {
//                $('ul#reloadNotificationRight > li.rm').last().remove();
//            }
//            var arrThongTinNguoiGui = thongtinnguoigui.split('[--]');
//            var itemAdd = '<li class="media py-3 border-bottom rm"><div class="image-box ratio-1-1 view overlay rounded-circle mr-2" style="width: 50px"><div class="image" style="' + arrThongTinNguoiGui[0] + '"></div><a><div class="mask rgba-white-slight"></div></a></div><div class="media-body"><p class="mt-0 mb-0"><a class="text-truncate d-block" href="javascript://">' + arrThongTinNguoiGui[1] + '</a></p><small class="font-italic text-truncate d-block w-100"><i class="fal fa-user-tag mr-1"></i>' + arrThongTinNguoiGui[2] + '</small><p class="mb-1 font-weight-bolder"><a href="/ThongBao/Detail/' + arrThongTinNguoiGui[5] + '">' + arrThongTinNguoiGui[3] + '</a></p><p class="mb-1"><small><i class="fal fa-clock mr-2"></i>' + arrThongTinNguoiGui[4] + '</small></p></div></li>'
//            $('ul#reloadNotificationRight').prepend(itemAdd);
//        }
//    };
//    $.connection.hub.start().done(function () {
//        $('.addNotificationRight').click(function () {
//            tbao.server.uploadNotificationRight($('#lstidThongBao').val());
//        });
//    });
//}
function UpdateFillNewestNotification() {
	$.ajax({
		type: "POST",
		url: '/ThongBao/UpdateFillNewestNotification',
		data: { lstid: $('#lstidDaNhanMoiNhat').val() },
		dataType: "html",
		success: function (res) {
		},
		error: function (response) {
		}
	});
}
function onDaXem(id, link, position) {
	$.ajax({
		type: "POST",
		url: '/ThongBao/DaXem',
		data: { id: id, link: link, position: position },
		dataType: "html",
		success: function (res) {
			res = JSON.parse(res);
			if (res.link != '') {
				window.location.href = res.link;
			} else {
				if (res.position == 'bell') {
					$("#navbarDropdownMenuLink").click();
				} else {
					toastr.warning('Hiện tại chưa có điều hướng trang.');
				}
			}
		},
		error: function (response) {
		}
	});
}
function LoadDataNotification(act) {
	$.ajax({
		type: "GET",
		url: '/Home/LoadNotificationBell',
		data: { pageIndex: $('#pageIndexLoad').val(), pageSize: 10, act: act },
		dataType: "html",
		success: function (res) {
			res = JSON.parse(res);
			if (res.html != '') {
				if (res.act == 'loadmore')
					$('#reloadNotification').append(res.html);
				else
					$('#reloadNotification').html(res.html);
				$('#pageIndexLoad').val(res.pageIndex);
				$('#totalItemLoad').val(res.total);
			}
		},
		error: function (response) {
		}
	});
}
//function LoadDataNotificationRight() {
//	$.ajax({
//		type: "GET",
//		url: '/Home/LoadNotificationRight',
//		data: {},
//		dataType: "html",
//		success: function (res) {
//			res = JSON.parse(res);
//			if (res.html != '') {
//				var lixemthem = '<li class="media text-right pt-3"><a class="d-inline-block" href="/ThongBao?tab=1">Xem thêm <i class="fal fa-angle-double-right"></i></a></li>';
//				$('#reloadNotificationRight').html(res.html + lixemthem);
//			} else {
//				$('#reloadNotificationRight').html('Không có thông báo mới...');
//			}
//		},
//		error: function (response) {
//		}
//	});
//}
//function LoadDataLichLamViec() {
//    $.ajax({
//        type: "GET",
//        url: '/Home/LichLamViecLanhDao',
//        data: {},
//        dataType: "html",
//        success: function (res) {
//            res = JSON.parse(res);
//            if (res.html != '') {
//                var lixemthem = '<li class="list-group-item px-0"><a href="/LichLanhDao/IndexView">Xem thêm <i class="fal fa-angle-double-right"></i></a></li>';
//                $('#reloadLichLamViecLanhDao').html(res.html + lixemthem);
//            } else {
//            	$('#reloadLichLamViecLanhDao').html('Không có lịch mới...');
//            }
//        },
//        error: function (response) {
//        }
//    });
//}
$('#reloadNotification').on('scroll', function () {
	var tongSoTrang = $('#totalItemLoad').val() / 10;
	if ($('#totalItemLoad').val() % 10 != 0) tongSoTrang += 1;
	if ($('#pageIndexLoad').val() <= tongSoTrang && $('#reloadNotification div.toast').length <= 100) {
		let div = $(this).get(0);
		if (div.scrollTop + div.clientHeight >= div.scrollHeight) LoadDataNotification('loadmore');
	}
});

