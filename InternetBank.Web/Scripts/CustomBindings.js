ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor)
    {
        var options = allBindingsAccessor().datepickerOptions || {},
            $el = $(element);

        //initialize datepicker with some optional options
        $el.datepicker(options).mask("99/99/9999", { placeholder: "mm/dd/yyyy" });

        //handle the field changing
        ko.utils.registerEventHandler(element, "change", function ()
        {
            var observable = valueAccessor();
            observable($el.datepicker("getDate"));
        });

        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function ()
        {
            $el.datepicker("destroy");
        });

    },
    update: function (element, valueAccessor)
    {
        var value = ko.utils.unwrapObservable(valueAccessor()),
            $el = $(element),
            current = $el.datepicker("getDate");

        if (value - current !== 0)
        {
            $el.datepicker("setDate", value);
        }
    }
};

ko.bindingHandlers.dialog = {
    init: function (element, valueAccessor, allBindingsAccessor)
    {
        var options = ko.utils.unwrapObservable(valueAccessor()) || {};
        //do in a setTimeout, so the applyBindings doesn't bind twice from element being copied and moved to bottom
        setTimeout(function ()
        {
            options.close = function ()
            {
                allBindingsAccessor().dialogVisible(false);
            };

            $(element).dialog(options);
        }, 0);

        //handle disposal (not strictly necessary in this scenario)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function ()
        {
            $(element).dialog("destroy");
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor)
    {
        var shouldBeOpen = ko.utils.unwrapObservable(allBindingsAccessor().dialogVisible),
            $el = $(element),
            dialog = $el.data("uiDialog") || $el.data("dialog");

        //don't call open/close before initilization
        if (dialog)
        {
            $el.dialog(shouldBeOpen ? "open" : "close");
        }
    }
};

ko.bindingHandlers.masked = {
    init: function (element, valueAccessor, allBindingsAccessor)
    {
        var all = ko.toJS(valueAccessor()),
            $el = $(element);

        if (all.mask)
        {
            if (all.isRegex)
                $el.mask("R", { definitions: { "R": all.mask } });
            else
                $el.mask(all.mask, { definitions: all.definitions, placeholder: all.placeholder });
        }
    },
    update: function (element, valueAccessor)
    {
        var all = ko.toJS(valueAccessor()),
            $el = $(element);

        if (all.mask)
        {
            if (all.isRegex)
                $el.mask("R", { definitions: { "R": all.mask } });
            else
                $el.mask(all.mask, { definitions: all.definitions, placeholder: all.placeholder });
        }
        else
            $el.unmask();
    }
};

ko.bindingHandlers.spinner = {
    init: function (element, valueAccessor, allBindingsAccessor)
    {
        var all = ko.toJS(valueAccessor());
        var options = all.options || {},
            $el = $(element);

        options.change = function (event, ui)
        {
            valueAccessor().value($el.spinner("value"));
        };

        $el.spinner(options);

        this.v = function (value, defaultValue) { return value === undefined ? defaultValue : value; };

        $el.spinner("option", "max", this.v(options.max, 999999999));
        $el.spinner("option", "min", this.v(options.min, -999999999));
        $el.spinner("option", "step", Math.pow(10, -this.v(options.precision, 0)));
        $el.spinner("option", "numberFormat", "N" + this.v(options.precision, 0));
        $el.spinner("option", "incremental", true);
        $el.spinner("option", "disabled", this.v(options.disabled, false));
        $el.spinner("value", this.v(all.value, options.default));

        ko.utils.domNodeDisposal.addDisposeCallback(element, function ()
        {
            $el.remove();
        });
    },
    update: function (element, valueAccessor)
    {
        var all = ko.toJS(valueAccessor());
        var options = all.options || {},
            $el = $(element);

        $el.spinner("option", "max", this.v(options.max, 999999999));
        $el.spinner("option", "min", this.v(options.min, -999999999));
        $el.spinner("option", "disabled", this.v(options.disabled, false));
        $el.spinner("value", this.v(all.value, options.default));
    }
};

ko.bindingHandlers.jqTree = {
    init: function (element, valueAccessor, allBindingsAccessor)
    {
        var options = ko.utils.unwrapObservable(valueAccessor()),
            $el = $(element);

        //initialize datepicker with some optional options

        ////handle the field changing
        //ko.utils.registerEventHandler(element, "change", function ()
        //{
        //    var observable = valueAccessor();
        //    observable($el.datepicker("getDate"));
        //});

        ////handle disposal (if KO removes by the template binding)
        //ko.utils.domNodeDisposal.addDisposeCallback(element, function ()
        //{
        //    $el.datepicker("destroy");
        //});

        $el.tree({
            data: options.dataSource() || [],
            dragAndDrop: true,
            selectable: true,
            autoOpen: true,
            onCanMoveTo: function(moved_node, target_node, position)
            {
                if (typeof options.onTreeNodeMoved == "function")
                    return options.onTreeNodeCanMoved(moved_node, target_node, position);
                return true;
            }
        });
        $el.bind(
            'tree.move',
            function (event)
            {
                if (typeof options.onTreeNodeMoved == "function")
                    options.onTreeNodeMoved(event);
                //console.log('moved_node', event.move_info.moved_node);
                //console.log('target_node', event.move_info.target_node);
                //console.log('position', event.move_info.position);
                //console.log('previous_parent', event.move_info.previous_parent);
            }
        );
        $el.bind(
            'tree.click',
            function (event)
            {
                var selected = $('li.jqtree-selected', $el);
                if (selected.length > 0 && selected[0] == event.node.element)
                    event.preventDefault();
                else
                {
                    if (options.selectedNodeInfo)
                    {
                        if (ko.isObservable(options.selectedNodeInfo.node))
                            valueAccessor().selectedNodeInfo.node(event.node);
                        else
                            valueAccessor().selectedNodeInfo.node = event.node;
                    }
                    selected.removeClass('jqtree-selected');
                }
                if (typeof options.onTreeNodeClicked == "function")
                    options.onTreeNodeClicked(event);
            }
        );
    },
    update: function (element, valueAccessor)
    {
        var options = ko.utils.unwrapObservable(valueAccessor()),
            $el = $(element);

        $el.tree('loadData', options.dataSource());
        if (options.selectedNodeInfo)
        {
            if (ko.isObservable(options.selectedNodeInfo.node))
                valueAccessor().selectedNodeInfo.node(null);
            else
                valueAccessor().selectedNodeInfo.node = null;
        }
    }
};