/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.filebrowserBrowseUrl = "/Content/asset/plugin/ckfinder/ckfinder.html";
    config.filebrowserImageUrl = "/Content/asset/plugin/ckfinder/ckfinder.html?type=Images";
    config.filebrowserFlashUrl = "/Content/asset/plugin/ckfinder/ckfinder.html?type=Flash";
    config.filebrowserUploadUrl = "/Content/asset/plugin/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files";
    config.filebrowserImageUploadUrl = "/Content/asset/plugin/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images";
    config.filebrowserFlashUploadUrl = "/Content/asset/plugin/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Flash";


    

    config.extraPlugins = 'youtube';
    config.youtube_responsive = true;
    config.height = 100;  
    config.extraPlugins += (config.extraPlugins.length == 0 ? '' : ',') + 'ckeditor_wiris';
    config.allowedContent = true;
};
