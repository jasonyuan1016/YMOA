;(function($){
/**
 * jqGrid Chinese Translation for v4.2
 * henryyan 2011.11.30
 * http://www.wsria.com
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
 * 
 * update 2011.11.30
 *		add double u3000 SPACE for search:odata to fix SEARCH box display err when narrow width from only use of eq/ne/cn/in/lt/gt operator under IE6/7
**/
$.jgrid = $.jgrid || {};
$.extend($.jgrid,{
	defaults : {
        recordtext: "Showing {0} - {1} records\u3000 retrieved {2} records",	// 共字前是全角空格
        emptyrecords: "Empty records",
		loadtext: "Loading..",
		pgtext : " {0} total {1} pages"
	},
	search : {
		caption: "Searching..",
		Find: "Find",
		Reset: "Reset",
		odata : ['=\u3000\u3000', '!=\u3000\u3000', '<\u3000\u3000', '<=','>\u3000\u3000','>=', 
			'开始于','不开始于','属于\u3000\u3000','不属于','结束于','不结束于','包含\u3000\u3000','不包含','空值于\u3000\u3000','非空值'],
		groupOps: [	{ op: "AND", text: "ALl" },	{ op: "OR",  text: "ANY" }	],
		matchText: " Match",
		rulesText: "Rules"
	},
	edit : {
        addCaption: "addCaption",
        editCaption: "editCaption",
        bSubmit: "Submit",
        bCancel: "Cancel",
        bClose: "Close",
		saveData: "Save Data？",
		bYes : "Yes",
		bNo : "No",
		bExit : "Exit",
		msg: {
            required:"Required",
			number:"Number",
            minValue:"minValue ",
            maxValue:"maxValue ",
            email: "Incorrect Email format",
            integer: "integer",
            date: "date",
            url: "Incorrect Url format ('http://' or 'https://')",
            nodefined: " nodefined！",
            novalue: " novalue！",
            customarray: "customarray！",
			customfcheck : "Custom function should be present in case of custom checking!"
			
		}
	},
	view : {
        caption: "caption",
        bClose: "Close:"
	},
	del : {
		caption: "Delete",
		msg: "Delete All？",
		bSubmit: "Delete",
		bCancel: "Cancel"
	},
	nav : {
		edittext: "",
		edittitle: "编辑所选记录",
		addtext:"",
		addtitle: "添加新记录",
		deltext: "",
		deltitle: "删除所选记录",
		searchtext: "",
		searchtitle: "查找",
		refreshtext: "",
		refreshtitle: "刷新表格",
		alertcap: "注意",
		alerttext: "请选择记录",
		viewtext: "",
		viewtitle: "查看所选记录"
	},
	col : {
		caption: "Selected",
		bSubmit: "submit",
		bCancel: "Cancel"
	},
	errors : {
		errcap : "Error",
		nourl : "no url",
        norecords: "norecords",
        model: "colNames and colModel are not the same length!"
	},
	formatter : {
		integer : {thousandsSeparator: " ", defaultValue: '0'},
		number : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, defaultValue: '0.00'},
		currency : {decimalSeparator:".", thousandsSeparator: " ", decimalPlaces: 2, prefix: "", suffix:"", defaultValue: '0.00'},
		date : {
			dayNames:   [
				"Sun", "Mon", "Tue", "Wed", "Thr", "Fri", "Sat",
		         "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
			],
			monthNames: [
				"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
				"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
			],
			AmPm : ["am","pm","AM","PM"],
			S: function (j) {return j < 11 || j > 13 ? ['st', 'nd', 'rd', 'th'][Math.min((j - 1) % 10, 3)] : 'th'},
			srcformat: 'Y-m-d',
			newformat: 'm-d-Y',
			masks : {
				ISO8601Long:"Y-m-d H:i:s",
				ISO8601Short:"Y-m-d",
				ShortDate: "Y/j/n",
				LongDate: "l, F d, Y",
				FullDateTime: "l, F d, Y g:i:s A",
				MonthDay: "F d",
				ShortTime: "g:i A",
				LongTime: "g:i:s A",
				SortableDateTime: "Y-m-d\\TH:i:s",
				UniversalSortableDateTime: "Y-m-d H:i:sO",
				YearMonth: "F, Y"
			},
			reformatAfterEdit : false
		},
		baseLinkUrl: '',
		showAction: '',
		target: '',
		checkbox : {disabled:true},
		idName : 'id'
	}
});
})(jQuery);
