/*
Copyright (c) 2003-2011, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function(config)
{
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';
    CKFinder.setupCKEditor(null, '../ckfinder/'); // 在 CKEditor 中集成 CKFinder，注意 ckfinder 的路径选择要正确。
};
