//(function() {
//    $(document).ready(function () {
//        function loadComments(endpoint, nid, btn) {
//            var _this = btn;
//            var holder = $('.eid-' + nid).find('.view-all-comments');
//            holder.html('<div class="comment-loading"><i class="fa fa-2x fa-spin fa-refresh"></i></div>');
//            $.get(endpoint, function (data) {
//                $(_this).addClass('comment-on');
//                var elem = $(data);
//                elem.find('.comments').click(function (e) {
//                    e.preventDefault();
//                    elem.remove();
//                    loadComments(endpoint + "&all=1", nid, btn);
//                });
              

//                elem.find('.btn-submit-comment').click(function (e) {
//                    e.preventDefault();

//                    // Save current state of submit button
//                    var btnSubmit = this;
//                    $(this).addClass('disabled');
//                    var curHtml = $(this).html();

//                    $(this).html('<i class="fa fa-spin fa-refresh"></i>');
//                    var parent = $(this).parent();
//                    var nid = parent.attr('data-nid');
//                    var content = parent.find('.comment-content').val();
//                    $.ajax({
//                        url: ViewHelper.getBaseUrl() + '/Ajax/Comment/?id=' + nid,
//                        data: { content: content, callbackId: 0 },
//                        type: "POST",
//                        success: function (data) {
//                            $(btnSubmit).removeClass('disabled');
//                            $(btnSubmit).html(curHtml);
//                            $(elem).find('.no-comments').hide();
//                            var newData = $(data)
//                            $(elem).find('.new-comments').append(newData);
//                            parent.find('.comment-content').val('');
//                            setupCommentButton(newData.find('.btn-comment'));
//                            var curComment = parseInt($(_this).find('span').html());
//                            $(_this).find('span').html(++curComment);
//                        }
//                    });
//                });
//                holder.html(elem);
//                setupCommentButton(holder.find('.btn-comment'));
//            });
//        }

//        // setup comment buttons
//        function setupCommentButton($selector) {
//            $($selector).click(function (e) {
//                e.preventDefault();
//                var _this = this;
//                var nid = $(this).attr('data-nid');

//                var endpoint = ViewHelper.getBaseUrl() + '/Trip/Comments/?id=' + nid;
//                loadComments(endpoint, nid, _this);
//            });
//        }
//        setupCommentButton('.btn-comment');

//        $('.comments-holder').each(function(i, e) {
//            if ($(e).attr('data-eid') !== undefined) {
//                var _this = '';
//                var nid = $(e).attr('data-eid');

//                var endpoint = ViewHelper.getBaseUrl() + '/Trip/Comments/?id=' + nid;
//                loadComments(endpoint, nid, _this);
//            }
//        });

       
//    });
//})();