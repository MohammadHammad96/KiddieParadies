sandtrapValidation = {
    getDependentElement: function (validationElement, dependentProperty) {
        var dependentElement = $('#' + dependentProperty);
        if (dependentElement.length === 1) {
            return dependentElement;
        }
        var name = validationElement.name;
        var index = name.lastIndexOf(".") + 1;
        var id = (name.substr(0, index) + dependentProperty).replace(/[\.\[\]]/g, "_");
        dependentElement = $('#' + id);
        if (dependentElement.length === 1) {
            return dependentElement;
        }
        // Try using the name attribute
        name = (name.substr(0, index) + dependentProperty);
        dependentElement = $('[name="' + name + '"]');
        if (dependentElement.length > 0) {
            return dependentElement.first();
        }
        return null;
    }
}

$(function () {
    jQuery.validator.addMethod('daterange', function (value, element, params) {
        if (!value) return false;
        var minDate = new Date(params.min);
        var maxDate = new Date(params.max);
        minDate.setHours(0, 0, 0, 0);
        maxDate.setHours(0, 0, 0, 0);
        value = new Date(value);
        value.setHours(0, 0, 0, 0);

        return value >= minDate && value <= maxDate;
    });

    jQuery.validator.unobtrusive.adapters.add('daterange', ['min', 'max'], function (options) {
        var params = {
            min: options.params.min,
            max: options.params.max
        };
        options.rules['daterange'] = params;
        if (options.message) {
            options.messages['daterange'] = options.message;
        }
    });
}(jQuery));

$(function () {
    jQuery.validator.addMethod('lessthandatetime', function (value, element, params) {
        var isValid = true;
        $.each(params.otherproperties, function (index, item) {
            if (new Date($(this).val()) >= new Date(value)) {
                isValid = false;
            }
        });
        return isValid;
    });

    jQuery.validator.unobtrusive.adapters.add('lessthandatetime', ['otherproperties'], function (options) {
        var element = options.element;
        var otherNames = options.params.otherproperties.split(',');
        var otherProperties = [];
        $.each(otherNames, function (index, item) {
            otherProperties.push(sandtrapValidation.getDependentElement(element, item))
        });
        options.rules['lessthandatetime'] = {
            otherproperties: otherProperties
        };
        options.messages['lessthandatetime'] = options.message;
    });
}(jQuery));



$(function () {
    jQuery.validator.addMethod('requiretypeahead', function (value, element, params) {
        var isValid = true;
        $.each(params.otherproperties, function (index, item) {
            if (($(this).val() == '') || ($(this).val() == '0') || ($(this).val() == 0)) {
                isValid = false;
            }
        });
        return isValid;
    });

    jQuery.validator.unobtrusive.adapters.add('requiretypeahead', ['otherproperties'], function (options) {
        var element = options.element;
        var otherNames = options.params.otherproperties.split(',');
        var otherProperties = [];
        $.each(otherNames, function (index, item) {
            otherProperties.push(sandtrapValidation.getDependentElement(element, item))
        });
        options.rules['requiretypeahead'] = {
            otherproperties: otherProperties
        };
        options.messages['requiretypeahead'] = options.message;
    });
}(jQuery));


$(function () {
    jQuery.validator.addMethod('requireimage', function (value, element, params) {
        var isValid = true;
        if ((value != '') && (value != null) && (value != undefined))
            return true;

        if (($(params.classidproperty).val() == '') ||
            ($(params.classidproperty).val() == null) ||
            ($(params.classidproperty).val() == undefined) ||
            ($(params.classidproperty).val() == '0'))
            isValid = false;
        return isValid;
    });

    jQuery.validator.unobtrusive.adapters.add('requireimage', ['classidproperty'], function (options) {
        var element = options.element;
        var classidporpertyName = options.params.classidproperty;
        var classidproperty = sandtrapValidation.getDependentElement(element, classidporpertyName);
        options.rules['requireimage'] = {
            classidproperty: classidproperty
        };
        options.messages['requireimage'] = options.message;
    });
}(jQuery));
