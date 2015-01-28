;(function($){
	$.fn.hcheckbox=function(options){
		$(':checkbox+label',this).each(function(){
			$(this).addClass('checkbox');
            if($(this).prev().is(':disabled')==false){
                if($(this).prev().is(':checked'))
				    $(this).addClass("checked");
            }else{
                $(this).addClass('disabled');
            }
		}).click(function(event){
				if(!$(this).prev().is(':checked')){
				    $(this).addClass("checked");
                    $(this).prev()[0].checked = true;
                }
                else{
                    $(this).removeClass('checked');			
                    $(this).prev()[0].checked = false;
                }
                event.stopPropagation();
			}
		).prev().hide();
	}

    $.fn.hradio = function(options){
        var self = this;
        return $(':radio+label',this).each(function(){
            $(this).addClass('hRadio');
            if($(this).prev().is("checked"))
                $(this).addClass('hRadio_Checked');
        }).click(function(event){
            $(this).siblings().removeClass("hRadio_Checked");
            if(!$(this).prev().is(':checked')){
	            $(this).addClass("hRadio_Checked");
                $(this).prev().prop("checked",true);
            }
               
            event.stopPropagation();
        })
        .prev().hide();
    }
})(jQuery)


$(function(){
	$('#chklist').hcheckbox();
	$('#radiolist').hradio();
	$('#btnOK').click(function(){
		var checkedValues = new Array();
		$('#chklist :checkbox').each(function(){
			if($(this).is(':checked'))
			{
				checkedValues.push($(this).val());
			}
		});

		alert(checkedValues.join(','));
		alert($('#radiolist :checked').val());
	})
});

$(function(){
	$('#radiolist1').hradio();
	$('#btnOK').click(function(){
		var checkedValues = new Array();
		$('#chklist :checkbox').each(function(){
			if($(this).is(':checked'))
			{
				checkedValues.push($(this).val());
			}
		});

		alert(checkedValues.join(','));
		alert($('#radiolist1 :checked').val());
	})
});



$(function(){
	$('#radiolist2').hradio();
	$('#btnOK').click(function(){
		var checkedValues = new Array();
		$('#chklist :checkbox').each(function(){
			if($(this).is(':checked'))
			{
				checkedValues.push($(this).val());
			}
		});

		alert(checkedValues.join(','));
		alert($('#radiolist2 :checked').val());
	})
});


$(function(){
	$('#radiolist3').hradio();
	$('#btnOK').click(function(){
		var checkedValues = new Array();
		$('#chklist :checkbox').each(function(){
			if($(this).is(':checked'))
			{
				checkedValues.push($(this).val());
			}
		});

		alert(checkedValues.join(','));
		alert($('#radiolist3 :checked').val());
	})
});