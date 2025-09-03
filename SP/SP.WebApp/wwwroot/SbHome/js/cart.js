$(document).ready(function() {
    // Add cart overlay to body
    $('body').append('<div class="cart-overlay"></div>');
    
    // Load cart content
    function loadCart() {
        $.get('/Cart/GetCartPartial', function(response) {
            if ($('.cart-sidebar').length === 0) {
                $('body').append(response);
            } else {
                $('.cart-sidebar').replaceWith(response);
            }
        });
    }

    // Toggle cart sidebar
    $(document).on('click', '.cart-link', function(e) {
        e.preventDefault();
        $('.cart-sidebar, .cart-overlay').addClass('active');
    });

    // Close cart sidebar
    $(document).on('click', '.close-cart-sidebar, .cart-overlay', function() {
        $('.cart-sidebar, .cart-overlay').removeClass('active');
    });

    // Update quantity
    $(document).on('click', '.increase-quantity, .decrease-quantity', function() {
        var $button = $(this);
        var $input = $button.siblings('.item-quantity');
        var currentValue = parseInt($input.val());
        var itemId = $button.data('id');
        
        if ($button.hasClass('increase-quantity')) {
            currentValue++;
        } else {
            currentValue = Math.max(1, currentValue - 1);
        }
        
        $.post('/Cart/UpdateQuantity', { 
            id: itemId, 
            quantity: currentValue 
        }, function(response) {
            if (response.success) {
                loadCart();
                updateCartCount(response.cartCount);
            }
        });
    });

    // Remove item from cart
    $(document).on('click', '.remove-item', function() {
        var itemId = $(this).data('id');
        
        $.post('/Cart/RemoveItem', { 
            id: itemId 
        }, function(response) {
            if (response.success) {
                loadCart();
                updateCartCount(response.cartCount);
            }
        });
    });

    // Update cart count in header
    function updateCartCount(count) {
        $('#cart-count').text(count);
    }

    // Handle quantity input changes
    $(document).on('change', '.item-quantity', function() {
        var $input = $(this);
        var itemId = $input.closest('.cart-item').data('id');
        var quantity = parseInt($input.val());
        
        if (quantity < 1) {
            quantity = 1;
            $input.val(1);
        }
        
        $.post('/Cart/UpdateQuantity', { 
            id: itemId, 
            quantity: quantity 
        }, function(response) {
            if (response.success) {
                loadCart();
                updateCartCount(response.cartCount);
            }
        });
    });
}); 