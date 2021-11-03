var FormFileUpload = function () {
    return {

        //main function to initiate the module
    	init: function (changeName, type, result, input, url, auto, name, multiple, value, setname, valueName, setSize, valueSize, fileType, intFolder) {
    		
            // setname = "false";
            var maxNumberOfFiles = 1;
            if (multiple == "True") { maxNumberOfFiles = 100; }
            var form = $("#" + name).closest('form');
            if (!form) form = ".fileupload";
            if (!input) input = "input.files";
            if (intFolder == '' || intFolder == undefined) intFolder = '0';
            if (!url) url = "/JqueryUpload/UploadFiles?intFolder=" + intFolder;
            auto = auto == "True" ? true : false;   // de upload anh luon khi chon
            if (!result) result = ".result-table";
            var temp = "template";
            switch (type) {
                case "pdf":
                    fileType = /(\.|\/)(pdf)$/i;
                    temp = "document";
                    break;
                case "image":
                    fileType = /(\.|\/)(gif|jpe?g|png)$/i;
                    temp = "image";
                    break;
                case "video":
                    fileType = /(\.|\/)(m1v|m2v|avi|gl|mjpg|moov|mov|movie|mp2|mpa|mpe|mpeg|mpg|mv)$/i;
                    temp = "video";
                    break;
                case "audio":
                    fileType = /(\.|\/)(au|m2a|m3u|mid|midi|mod|mp3|voc|wav)$/i;
                    temp = "audio";
                    break;
                case "document":
                default:
                    if (fileType == null || fileType == "") {
                        fileType =
                            /(\.|\/)(mp4|m1v|m2v|avi|gl|mjpg|moov|mov|movie|mp2|mpa|mpe|mpeg|mpg|mv|gif|jpe?g|png|au|m2a|m3u|mid|midi|mod|mp3|voc|wav|xl|xla|xls|xlsx|doc|docx|ppt|pptx|txt|pdf)$/i;
                    } else {
                        if (fileType == "video") {
                            fileType = /(\.|\/)(m1v|m2v|avi|gl|mjpg|moov|mov|movie|mp4|mp2|mpa|mpe|mpeg|mpg|mv)$/i;
                        }
                    }
                    temp = "document";
                    break;
                    //default:
                    //    fileType = /(\.|\/)(m1v|m2v|avi|gl|mjpg|moov|mov|movie|mp2|mpa|mpe|mpeg|mpg|mv|gif|jpe?g|png|au|m2a|m3u|mid|midi|mod|mp3|voc|wav|xl|xla|xls|xlsx|doc|docx|ppt|pptx|txt|pdf)$/i;
                    //    temp = "template";
                    //    break;
            }
            // in form edit
            if (value != null && value != "") {
                FormFileUpload.showfile_uploaded(temp, value, valueName, setname, valueSize, changeName);
            }
            $(form).fileupload({
                disableImageResize: false,
                fileInput: $(input),
                autoUpload: auto,
                maxNumberOfFiles: maxNumberOfFiles,
                url: url,
                filesContainer: result,
                uploadTemplateId: temp + "-upload",
                downloadTemplateId: temp + "-download",
                disableImageResize: /Android(?!.*Chrome)|Opera/.test(window.navigator.userAgent),
                acceptFileTypes: fileType,
                maxFileSize: 2500000000,//1500000000
                send: function (e, data) {
                    var abc = data.context.find('input[name="abcd"]').val();
                    data.files[0].nameFile = abc;
                },
                finished: function (e, data) {
                    
                    var nameFile = data.files[0].nameFile;
                    data.context.find('input[name="abcd"]').val(nameFile);
                    if (data != null) {
                        var idInput = $(input).attr("data-id");
                        var listFile = $("#" + idInput).val();
                        var lstSetName = $("#AttachmentName").val();
                        var lstFileSize = $("#AttachmentSize").val();
                        $.each(data.result.files, function (index, file) {
                            if (listFile != "") {
                                listFile += ",";
                                lstSetName += "|";
                                lstFileSize += "|";
                            }
                            listFile += file.url;
                            lstSetName += file.abcd;
                            lstFileSize += file.size;
                        });
                        $("#" + idInput).val(listFile);
                        $("#AttachmentName").val(lstSetName);
                        $("#AttachmentSize").val(lstFileSize);

                    }
                },
                destroyed: function (e, data) {
                    
                    var idInput = $(input).attr("data-id");//Attchment

                    var idInputDelete = $(input).attr("data-delete");
                    var listFile = $("#" + idInput).val();
                    var listFileDelete = $("#" + idInputDelete).val();
                    var dataDelete = $(data.context.context).attr("data-delete");
                    if (listFileDelete != "") {
                        listFileDelete += ","
                    }

                    listFileDelete += dataDelete;
                    $("#" + idInputDelete).val(listFileDelete);
                    var listFileNew = "";
                    var listNameFileNew = "";
                    var listSizeNew = "";

                    var arr = listFile.split(',');
                    $.each(arr, function (index, file) {
                        if (file != dataDelete) {
                            if (listFileNew != "") {
                                listFileNew += ","
                            }
                            listFileNew += file;
                        }
                    });
                    $("#" + idInput).val(listFileNew);
                }
            });
        },
        showfile_uploaded: function (temp, value, valueName, setName, valueSize, changeName) {
            
            var arrFiles = [];
            var arrFileName = [];
            var arrFileSize = [];
            var arrFileReplaceName = [];
            if (value.indexOf(',') > -1) {
                arrFiles = value.split(",");
                arrFileName = valueName.split("|");
                arrFileSize = valueSize.split("|");
                arrFileReplaceName = setName.split("|");
            } else {
                arrFiles[0] = value;
                arrFileName[0] = valueName;
                arrFileSize[0] = valueSize;
                arrFileReplaceName[0] = setName;
            }
            var strFiles = "";
            var nameFile = "";
            var nameshow = "";
            var attchmentName = "";
            var attchmentSize = "";

            switch (temp) {
                case "image":
                    for (var i = 0; i < arrFiles.length; i++) {
                        if (arrFiles[i] != '') {
                            
                            nameFile = arrFiles[i].substring(arrFiles[i].lastIndexOf('/') + 1);
                            attchmentSize = arrFileSize[i];
                            strFiles += '<tr class="template-download">';
                            strFiles += '        <td >';
                            strFiles += '            <p class="name"><div id="previewthumbnail"><a id="aTag" class="delete" href="#" title="Xóa ảnh" data-type="GET" data-url="/JqueryUpload/DeleteFile?url=' + arrFiles[i] + '" data-delete="' + arrFiles[i] + '"><i class="fa fa-times" style="font-size: 25px;"></i></a>';
                            strFiles += '                <a href="' + arrFiles[i] + '" title="' + nameFile + '" download="' + nameFile + '" >';
                            strFiles += '                        <img src="' + arrFiles[i] + '"width="200">'
                        	strFiles += '               </a>';
                            //strFiles += '               </a><button class="btn btn-danger delete" data-type="GET" data-url="/JqueryUpload/DeleteFile?url=' + nameFile + ' " data-delete="/Images/' + nameFile + '" >Xóa</button>';
                            strFiles += '                   <input type="hidden" name="linkFile" value="' + arrFiles[i] + '" />';
                            strFiles += '            </div></p>';
                            strFiles += '        </td>';
                            //strFiles += '        <td>';
                            //strFiles += '             <p class="size" id="size" name = "size">' + attchmentSize + '</p>';
                            //strFiles += '               <input type="hidden" name="sizeFile" value="' + attchmentSize + '" />';
                            //strFiles += '             <div class="progress progress-striped active" role="progressbar" aria-valuemin="0" aria-valuemax="100" aria-valuenow="0">';
                            //strFiles += '                   <div class="progress-bar progress-bar-success" style="width:0%;"></div>';
                            //strFiles += '             </div>';
                            //strFiles += '       </td>';
                            //strFiles += '         <td>';
                            //strFiles += '          <button class="btn btn-danger delete" data-type="GET" data-url="/JqueryUpload/DeleteFile?url=' + nameFile + ' " data-delete="/Images/' + nameFile + '" >Xóa</button>';
                            //strFiles += '         </td>';
                            strFiles += '    </tr>';
                        }
                    }
                    break;
                default:
                    
                    for (var i = 0; i < arrFiles.length; i++) {
                        
                        if (arrFiles[i] != '') {
                            nameFile = arrFiles[i].substring(arrFiles[i].lastIndexOf('/') + 1);
                            attchmentName = arrFileName[i];
                            attchmentSize = arrFileSize[i];
                            nameshow = nameFile.substring(15);
                            if (valueName != "") {
                                nameshow = valueName;
                            }

                            strFiles += '<tr class="template-download">';
                            strFiles += '        <td >';
                            strFiles += '            <p class="name tal">';
                            strFiles += '                <a href="' + arrFiles[i] + '"class="text-primary h-tdu" title="' + nameFile + '" download="' + nameFile + '" >' + attchmentName + '</a>';
                            strFiles += '                   <input type="hidden" name="nameFile" value="' + arrFileName[i] + '" />';
                            strFiles += '                   <input type="hidden" name="linkFile" value="' + arrFiles[i] + '" />'; 
                            strFiles += '                <a class="viewFile text-primary" data-link="' + arrFiles[i] + '" target="_blank" style="margin-left: 5px; color: #337ab7"><i class="fa fa-search"></i> Xem </a>';
                            strFiles += '            </p>';
                        	//'" href="/Home/ViewFile?linkdown=' + arrFiles[i] +
                            strFiles += '        </td>';
                            //if (changeName == "True") {
                            //    strFiles += '        <td>';
                            //    strFiles += '             <input class="form-control valid" type="text" id="abcd" name="abcd" value="' + arrFileReplaceName[i] + '" />';
                            //    strFiles += '        </td>';
                            //}
                            strFiles += '        <td>';
                            strFiles += '             <p class="size" id="size" name = "size">' + attchmentSize + '</p>';
                            strFiles += '               <input type="hidden" name="sizeFile" value="' + attchmentSize + '" />';
                            strFiles += '             <div class="progress progress-striped active" role="progressbar" aria-valuemin="0" aria-valuemax="100" aria-valuenow="0">';
                            strFiles += '                   <div class="progress-bar progress-bar-success" style="width:0%;"></div>';
                            strFiles += '             </div>';
                            strFiles += '       </td>';
                            strFiles += '         <td>';
                            strFiles += '          <button title="Xóa" class="btn btn-danger btn-sm delete br-50pt" data-type="GET" data-url="/JqueryUpload/DeleteFile?url=' + arrFiles[i] + '" data-delete="' + arrFiles[i] + '"><span><i class="fal fa-times"></i></span></button>';
                            strFiles += '         </td>';
                            strFiles += '    </tr>';
                        }
                    }
                    break;
            }
            $("#table-edit").html(strFiles);
        },

    };

}();