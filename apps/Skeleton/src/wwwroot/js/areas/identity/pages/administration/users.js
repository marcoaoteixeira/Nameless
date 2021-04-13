(function ($) {
    $('#usersGrid').jsGrid({
        height: 'auto',
        width: '100%',

        filtering: true,
        sorting: true,
        paging: true,
        autoload: true,

        pageSize: 15,
        pageButtonCount: 5,

        deleteConfirm: 'Do you really want to delete the user?',

        controller: {
            loadData: function (filter) {
                return $.ajax({
                    type: 'GET',
                    url: '/Identity/api/v1/Administration/ListUsers',
                    data: filter,
                    dataType: 'json'
                });
            }
        },

        fields: [
            { title: 'ID', name: 'id', type: 'text', width: 50, visible: false, sorting: false, filtering: false },
            { title: 'Full name', name: 'fullName', type: 'text', width: 150 },
            { title: 'E-mail', name: 'email', type: 'text', width: 150 },
            { title: 'E-mail Confirmed', name: 'emailConfirmed', type: 'checkbox', sorting: false, filtering: false },
            { title: 'Is Locked?', name: 'isLocked', type: 'checkbox', sorting: false, filtering: false },
            { title: 'Lockout End Date', name: 'lockoutEndDate', type: 'text', sorting: false, filtering: false },
            { title: 'Two Factor Enabled', name: 'twoFactorEnabled', type: 'checkbox', sorting: false, filtering: false },
            { title: 'Access failed count', name: 'accessFailedCount', type: 'number', sorting: false, filtering: false },
            { type: 'control', editButton: false }
        ]
    });
})(jQuery);