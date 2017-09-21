Vue.component('pagination', {
    template: '<ul class="pagination pagination-sm no-margin pull-right"><li><a href="#" @click="goto(1)">&laquo;</a></li><template v-for="n in showpages"><li :class="n==page?\'active\':\'\'"><a href="#" tabindex="0" @click="goto(n)">{{n}}</a></li></template><li><a href="#" @click="goto(totalpage)">&raquo;</a></li></ul>',
    props: ['page', 'totalpage'],
    data: function () {
        return {

        }
    },
    methods: {
        goto: function (page) {
            if (this.page == page) {
                return false;
            } else {
                this.$emit("goto", page);
            }
        }
    },
    computed: {
        showpages: function () {
            var result = [];
            var minpage = Math.max(1, this.page - 3);
            var maxpage = Math.min(this.totalpage, minpage + 7);
            minpage = Math.max(1, maxpage - 7);
            for (var p = minpage; p <= maxpage; p++) {
                result.push(p);
            }
            return result;
        }
    }
});
Vue.http.options.emulateJSON = true;
Vue.http.interceptors.push(function (request, next) {
    var loadlayerid = layer.load(2);
    next(function (response) {
        // ...
        // 请求发送后的处理逻辑
        // ...
        layer.close(loadlayerid);
        console.log(response);
        if (response.body.errCode && response.body.errCode != 0) {
            layer.alert(response.body.errMsg, { icon: 5 });
        }
        // 根据请求的状态，response参数会返回给successCallback或errorCallback
        return response
    })
})
var filterdata = [];//表头过滤数据
var pageVue = Vue.extend({
    data: function () {
        return {
            currentindex: -1,
            currentitem: {},
            pagedata: {},
            page: 1,
            dataurl: "",
            querydata: {},//快速查询数据
            submiturl: "",
            allchecked: "",
            checkedids:[]
        }
    },
    watch: {
        currentindex: function (curVal, oldVal) {
            if (curVal >= 0) {
                this.currentitem = $.extend(true, {}, this.pagedata.Items[curVal]);
                console.log(this.currentitem);
            } else {
                this.currentitem = {};
            }
        },
        checkedids: {
            handler: function (curVal, oldVal) {
                this.allchecked = this.checkedids.length == this.pagedata.Items.length;
            },
            deep:true
        }
    },
    created: function () {

        this.loaddata();
    },
    mounted: function () {
        $(this.$el).removeClass("hide");
        //加载表头过滤器
        filter.initFilter($(this.$el));
    },
    computed: {
        totalpage: function () {
            return this.pagedata.TotalPages ? this.pagedata.TotalPages : 0;
        },
    },
    methods: {
        checkall:function(){
            if (!this.allchecked) {//实现反选
                this.checkedids = [];
            } else {//实现全选
                this.checkedids = [];
                for (var i = 0; i < this.pagedata.Items.length; i++) {
                    this.checkedids.push(this.pagedata.Items[i].Id);
                }
            }
        },
        thumb: function (filepath, width) {
            return "/data/thumbfile?filepath=" + encodeURIComponent( filepath) + "&w=" + width;
        },
        dateformat: function (value, fmt) {
            var result = "";
            if (value) {
                var date = new Date(value.replace('T', ' ').replace(/-/ig, "\/"));
                result = date.format(fmt ? fmt : "yyyy-MM-dd");
            }
            return result;
        },
        submit: function () {
            var _self = this;
            var data = $.extend(true, { action: "submit" }, this.currentitem);
            this.$http.post(this.dataurl+"submit", data).then(function (data) {
                data = data.body;
                if (data.errCode == 0) {
                    $('#submitform').modal('hide');
                    _self.loaddata();
                }
            });
            //$.post(this.submiturl, this.currentitem, function (data) {
            //    layer.closeAll();
            //    if (data.errCode == 0) {
            //        $('#submitform').modal('hide');
            //        _self.loaddata();
            //    } else {
            //        layer.alert(data.errMsg, { icon: 5 });
            //    }

            //}, 'json');
        },
        remove: function (id) {
            var _self = this;
            layer.confirm('确认删除此项？', {
                btn: ['确认', '取消'] //按钮
            }, function (index) {
                layer.close(index);
                var data = {id: id };
                _self.$http.post(_self.dataurl+"delete" ,data).then(function (data) {
                    data = data.body;
                    if (data.errCode == 0) {
                        _self.loaddata();
                    }
                })
            }, function () {

            });
        },
        loaddata: function () {
            var url = this.dataurl;
            if (!url) { return; }
            var filterstr = JSON.stringify(filterdata);
            var querydata = $.extend({ page: this.page, filters: filterstr }, this.querydata);
            var _self = this;

            this.$http({
                method: 'GET',
                url: url + "?" + $.param(querydata),
            }).then(function (data) {
                this.pagedata = data.body;
                this.page = this.pagedata.CurrentPage;
                });
            this.currentindex = -1;
            //this.$http.get(url, { page: this.page,pagesize:1 }).then(function (data) {
            //    this.pagedata = data.body;
            //    this.page = this.pagedata.CurrentPage;
            //    layer.closeAll();
            //});
            //$.get(url, { page: this.page }, function (data) {
            //    _self.pagedata = data;
            //    _self.page = data.CurrentPage;
            //    layer.closeAll();
            //}, 'json')
        },
        gotopage: function (page) {
            this.page = page;
            this.loaddata();
        },
        filterby: function (data) {
            console.log(data);
            this.querydata = $.extend(this.querydata, data);
            this.page = 1;
            this.loaddata();
        },
    }
})


/** * 对Date的扩展，将 Date 转化为指定格式的String * 月(M)、日(d)、12小时(h)、24小时(H)、分(m)、秒(s)、周(E)、季度(q)
   可以用 1-2 个占位符 * 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) * eg: * (new
   Date()).pattern("yyyy-MM-dd hh:mm:ss.S")==> 2006-07-02 08:09:04.423      
* (new Date()).pattern("yyyy-MM-dd E HH:mm:ss") ==> 2009-03-10 二 20:09:04      
* (new Date()).pattern("yyyy-MM-dd EE hh:mm:ss") ==> 2009-03-10 周二 08:09:04      
* (new Date()).pattern("yyyy-MM-dd EEE hh:mm:ss") ==> 2009-03-10 星期二 08:09:04      
* (new Date()).pattern("yyyy-M-d h:m:s.S") ==> 2006-7-2 8:9:4.18      
*/
Date.prototype.format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1, //月份         
        "d+": this.getDate(), //日         
        "h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12, //小时         
        "H+": this.getHours(), //小时         
        "m+": this.getMinutes(), //分         
        "s+": this.getSeconds(), //秒         
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度         
        "S": this.getMilliseconds() //毫秒         
    };
    var week = {
        "0": "/u65e5",
        "1": "/u4e00",
        "2": "/u4e8c",
        "3": "/u4e09",
        "4": "/u56db",
        "5": "/u4e94",
        "6": "/u516d"
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    if (/(E+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, ((RegExp.$1.length > 1) ? (RegExp.$1.length > 2 ? "/u661f/u671f" : "/u5468") : "") + week[this.getDay() + ""]);
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        }
    }
    return fmt;
}

/*
过滤器
*/
var filter = {
    initFilter: function ($el) {
        //表头
        $el.find("th[filterField]").each(function () {
            $("<i class='fa fa-filter'></i>").appendTo($(this)).click(function () {
                filter.buildFilterForm($(this).closest("th"));
                window.filterlayerid = layer.open({
                    type: 1,
                    shade: [0.5, '#000'],
                    area: ['370px', '290px'], //宽高
                    title: "过滤器",
                    content: $('#panel_filter'), //捕获的元素，注意：最好该指定的元素要存放在body最外层，否则可能被其它的相对元素所影响
                });
            })
        });

    },
    redrawFilter: function () {
        $(page.$el).find("th[filterField]").each(function () {
            $(this).find("i").attr("class", "fa fa-filter");
            var currentFilters = window.filterdata;
            for (var i = 0; i < currentFilters.length; i++) {

                if (currentFilters[i].filterField == $(this).attr("filterField")) {
                    $(this).find("i").attr("class", "fa fa-eye-slash")
                    break;
                }
            }
        })
    },
    filterConditions: {
        text: [{ text: "等于", value: "equal" }, { text: "不等于", value: "notequal" }, { text: "大于", value: "greaterthan" }, { text: "小于", value: "lessthan" }, { text: "大于等于", value: "greaterthanorequal" }, { text: "小于等于", value: "lessthanorequal" }, { text: "包含", value: "contain" }, { text: "不包含", value: "notcotain" }, { text: "以_开始", value: "startwith" }, { text: "以_结束", value: "endwith" }],
        number: [{ text: "等于", value: "equal" }, { text: "不等于", value: "notequal" }, { text: "大于", value: "greaterthan" }, { text: "小于", value: "lessthan" }, { text: "大于等于", value: "greaterthanorequal" }, { text: "小于等于", value: "lessthanorequal" }],
        date: [{ text: "等于", value: "equal" }, { text: "不等于", value: "notequal" }, { text: "大于", value: "greaterthan" }, { text: "小于", value: "lessthan" }, { text: "大于等于", value: "greaterthanorequal" }, { text: "小于等于", value: "lessthanorequal" }]
    },
    buildFilterForm: function (thead) {
        var title = $(thead).text();
        var filterType = $(thead).attr("filterType");
        var filterField = $(thead).attr("filterField");
        var strwhereFormat = $(thead).attr("strwhereFormat") || "";
        var table = $(thead).closest("tr").attr("table");
        $("#panel_filter select").html('');
        $("#panel_filter select[name='condition2']").append('<option value="">未设定</option>');
        $("#panel_filter").attr("filterType", filterType).attr("filterField", filterField).attr("strwhereFormat", strwhereFormat);
        $("#panel_filter caption").html(title);
        $("#panel_filter input[name='value1']").parent("td").html('').append('<input type="text" name="value1" class="textInput"/>');
        $("#panel_filter input[name='value2']").parent("td").html('').append('<input type="text" name="value2" class="textInput"/>');
        $("#panel_filter input[type='radio']").attr("checked", false).filter("[value='and']").attr("checked", true);
        $.each(filter.filterConditions[filterType], function (i, v) {
            $("#panel_filter select").append("<option value='" + v.value + "'>" + v.text + "</option>");
        });
        $("#panel_filter select[name='condition1']").append('<option value="">未设定</option>');
        //
        if (filterType == "date") {
            $("#panel_filter input[type='text']").attr("type", "date");
        }
        else if (filterType == "number") {
            $("#panel_filter input[type='text']").attr("type", "number");
        }
        else if (filterType == "text") {
            //如果没有查询字符串模板且未禁用自动完成，则文本类型的进行自动完成
            //var autotable = $(thead).attr("autotable");
            //if (!$(thead).attr("nocomplete") && (!strwhereFormat || autotable)) {
            //    autoComplete($("#panel_filter input[type='text']"), { table: autotable || table, field: filterField });
            //}
        }
        //绑定初始值
        var currentFilters = window.filterdata;
        for (var i = 0; i < currentFilters.length; i++) {
            if (currentFilters[i].filterField == filterField) {
                $("#panel_filter select[name='condition1']").val(currentFilters[i].condition1);
                $("#panel_filter select[name='condition2']").val(currentFilters[i].condition2);
                $("#panel_filter input[name='value1']").val(currentFilters[i].value1);
                $("#panel_filter input[name='value2']").val(currentFilters[i].value2);
                $("#panel_filter input[type='radio']").attr("checked", false).filter("[value='" + currentFilters[i].joiner + "']").attr("checked", true);
                break;
            }
        }

    },
    submitFilter: function () {
        var newFilter = {
            filterType: $("#panel_filter").attr("filterType"),
            filterField: $("#panel_filter").attr("filterField"),
            strwhereFormat: $("#panel_filter").attr("strwhereFormat") || "",
            condition1: $("#panel_filter").find("select[name='condition1']").val(),
            condition2: $("#panel_filter").find("select[name='condition2']").val(),
            value1: $("#panel_filter").find("input[name='value1']").val(),
            value2: $("#panel_filter").find("input[name='value2']").val(),
            joiner: $("#panel_filter").find("input[name='joiner']:checked").val(),
        };
        //是否已存在过滤，若有则替换原过滤，若无，则增加
        var findindex = -1;
        for (var i = 0; i < filterdata.length; i++) {
            if (filterdata[i].filterField == newFilter.filterField) {
                findindex = i;//找到的index
            }
        }
        if (findindex != -1) {
            //移除此项
            filterdata.splice(findindex, 1);
        }
        //如果当前过滤有内容，则重新添加此过滤
        if ((newFilter.condition1) || (newFilter.condition2)) {
            filterdata.push(newFilter);
        }
        layer.close(window.filterlayerid);
        page.loaddata();
        filter.redrawFilter();
    },
    clearFilter: function () {
        $("#panel_filter").find("select,input").not("[type='button']").not("[type='radio']").val('');
        filter.submitFilter();
    }
}