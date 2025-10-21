<script lang="ts">
    import { apiService, type Disc, DiscCategory } from '../api.js';
    import { createEventDispatcher } from 'svelte';

    export let disc: Disc;

    const dispatch = createEventDispatcher<{
        edit: Disc;
        delete: number;
        imageUpload: number;
        toggleBag: number;
        toggleForSale: number;
        toggleFavorite: number;
    }>();

    let showPositionControls = false;
    let imagePosition = { x: disc.imagePositionX || 50, y: disc.imagePositionY || 50 };
    let imageZoom = disc.imageZoom || 100;
    let isDragging = false;
    let imageElement: HTMLImageElement;
    let saveTimeout: number | null = null;

    function handleEdit() {
        dispatch('edit', disc);
    }

    function handleDelete() {
        if (confirm(`Are you sure you want to delete ${disc.name}?`)) {
            dispatch('delete', disc.id);
        }
    }

    function handleImageUpload() {
        dispatch('imageUpload', disc.id);
    }

    function handleToggleBag() {
        dispatch('toggleBag', disc.id);
    }

    function handleToggleForSale() {
        dispatch('toggleForSale', disc.id);
    }

    function handleToggleFavorite() {
        dispatch('toggleFavorite', disc.id);
    }

    function togglePositionControls() {
        showPositionControls = !showPositionControls;
        console.log(`Toggle positioning controls: ${showPositionControls ? 'ON' : 'OFF'} for ${disc.name}`);
    }

    function setImagePosition(x: number, y: number) {
        console.log(`Setting image position: x=${x}, y=${y} for ${disc.name}`);
        imagePosition = { x, y };
        updateImageStyle();
    }

    function setImageZoom(zoom: number) {
        const newZoom = Math.max(50, Math.min(200, zoom));
        console.log(`Setting image zoom: ${newZoom}% for ${disc.name}`);
        imageZoom = newZoom;
        updateImageStyle();
    }

    function updateImageStyle() {
        if (imageElement) {
            imageElement.style.objectPosition = `${imagePosition.x}% ${imagePosition.y}%`;
            imageElement.style.transform = `scale(${imageZoom / 100})`;
        }
        // Only save if we're in positioning mode and values have actually changed
        if (showPositionControls) {
            debouncedSave();
        }
    }

    function debouncedSave() {
        // Clear existing timeout
        if (saveTimeout) {
            clearTimeout(saveTimeout);
        }
        
        // Set new timeout to save after 1 second of no changes
        saveTimeout = setTimeout(async () => {
            try {
                await apiService.updateImagePosition(disc.id, imagePosition.x, imagePosition.y, imageZoom);
                // Update the local disc object to reflect saved state
                disc.imagePositionX = imagePosition.x;
                disc.imagePositionY = imagePosition.y;
                disc.imageZoom = imageZoom;
                console.log(`Saved image position for ${disc.name}: x=${imagePosition.x}, y=${imagePosition.y}, zoom=${imageZoom}`);
            } catch (error) {
                console.error('Failed to save image position:', error);
            }
        }, 1000);
    }

    // Initialize image style on mount
    import { onMount, onDestroy } from 'svelte';
    
    onMount(() => {
        // Set initial position from disc data
        imagePosition = { x: disc.imagePositionX || 50, y: disc.imagePositionY || 50 };
        imageZoom = disc.imageZoom || 100;
        
        // Apply the style after a brief delay to ensure element is ready
        setTimeout(() => {
            if (imageElement) {
                imageElement.style.objectPosition = `${imagePosition.x}% ${imagePosition.y}%`;
                imageElement.style.transform = `scale(${imageZoom / 100})`;
            }
        }, 10);
    });
    
    onDestroy(() => {
        if (saveTimeout) {
            clearTimeout(saveTimeout);
        }
    });

    function zoomIn() {
        setImageZoom(imageZoom + 10);
    }

    function zoomOut() {
        setImageZoom(imageZoom - 10);
    }

    function resetZoom() {
        setImageZoom(100);
    }

    function resetAll() {
        setImagePosition(50, 50);
        setImageZoom(100);
    }

    function handleImageMouseDown(event: MouseEvent) {
        if (!showPositionControls) return;
        event.preventDefault();
        isDragging = true;
        
        const target = event.currentTarget as HTMLImageElement;
        if (target) {
            const rect = target.getBoundingClientRect();
            updateImagePosition(event, rect);
        }
    }

    function handleImageMouseMove(event: MouseEvent) {
        if (!isDragging || !showPositionControls) return;
        event.preventDefault();
        
        const target = event.currentTarget as HTMLImageElement;
        if (target) {
            const rect = target.getBoundingClientRect();
            updateImagePosition(event, rect);
        }
    }

    function handleImageMouseUp() {
        isDragging = false;
    }

    function updateImagePosition(event: MouseEvent, rect: DOMRect) {
        const x = Math.max(0, Math.min(100, ((event.clientX - rect.left) / rect.width) * 100));
        const y = Math.max(0, Math.min(100, ((event.clientY - rect.top) / rect.height) * 100));
        setImagePosition(x, y);
    }

    function resetImagePosition() {
        setImagePosition(50, 50);
    }
</script>

<div class="disc-card" class:in-bag={disc.inTheBag} class:for-sale={disc.forSale} class:is-favorite={disc.isFavorite}>
    <div class="disc-image" class:positioning-mode={showPositionControls}>
        <!-- svelte-ignore a11y-no-noninteractive-element-interactions -->
        <img
            bind:this={imageElement}
            src={apiService.getImageUrl(disc.imagePath)} 
            alt={disc.name}
            style="object-position: {imagePosition.x}% {imagePosition.y}%;"
            class:draggable={showPositionControls}
            on:error={(e) => {
                const target = e.currentTarget as HTMLImageElement;
                if (target) target.src = '/placeholder-disc.png';
            }}
            on:mousedown={handleImageMouseDown}
            on:mousemove={handleImageMouseMove}
            on:mouseup={handleImageMouseUp}
            on:mouseleave={handleImageMouseUp}
        />
        <button class="favorite-btn" class:is-favorite={disc.isFavorite} on:click={handleToggleFavorite} title={disc.isFavorite ? 'Remove from favorites' : 'Add to favorites'}>
            {disc.isFavorite ? '❤️' : '🤍'}
        </button>
        <button class="image-upload-btn" on:click={handleImageUpload} title="Upload Image">
            📷
        </button>
        <button class="position-toggle-btn" on:click={togglePositionControls} title="Adjust Image Position">
            {showPositionControls ? '✅' : '🎯'}
        </button>
        
        {#if showPositionControls}
            <div class="position-controls">
                <div class="zoom-controls">
                    <button on:click={zoomOut} disabled={imageZoom <= 50} title="Zoom Out">🔍➖</button>
                    <span class="zoom-level">{imageZoom}%</span>
                    <button on:click={zoomIn} disabled={imageZoom >= 200} title="Zoom In">🔍➕</button>
                    <button on:click={resetAll} title="Reset Position & Zoom">🔄</button>
                </div>
                <div class="position-presets">
                    <button on:click={() => setImagePosition(50, 20)} title="Top Center">⬆️</button>
                    <button on:click={() => setImagePosition(50, 50)} title="Center">⭕</button>
                    <button on:click={() => setImagePosition(50, 80)} title="Bottom Center">⬇️</button>
                    <button on:click={() => setImagePosition(20, 50)} title="Left Center">⬅️</button>
                    <button on:click={() => setImagePosition(80, 50)} title="Right Center">➡️</button>
                </div>
                <div class="position-hint">
                    Drag to position • Use zoom buttons to resize
                </div>
            </div>
        {/if}
        
        {#if disc.inTheBag}
            <div class="bag-indicator" title="In the bag">
                🎒
            </div>
        {/if}
        {#if disc.forSale}
            <div class="sale-indicator" title="For sale">
                💰
            </div>
        {/if}
        {#if disc.isFavorite}
            <div class="favorite-indicator" title="Favorite">
                ❤️
            </div>
        {/if}
    </div>
    
    <div class="disc-info">
        <h3>{disc.name}</h3>
        <p class="brand">{disc.brand}</p>
        <p class="category">{apiService.getCategoryName(disc.category)}</p>
        <p class="flight-numbers">Flight: {disc.flightNumbers}</p>
        <p class="details">{disc.plastic} • {disc.color} • {disc.weight}g</p>
        {#if disc.description}
            <p class="description">{disc.description}</p>
        {/if}
    </div>
    
    <div class="actions">
        <div class="primary-actions">
            <button class="bag-btn" class:in-bag={disc.inTheBag} on:click={handleToggleBag} title={disc.inTheBag ? 'Remove from bag' : 'Add to bag'}>
                {disc.inTheBag ? '📤' : '🎒'}
            </button>
            <button class="sale-btn" class:for-sale={disc.forSale} on:click={handleToggleForSale} title={disc.forSale ? 'Remove from sale' : 'Mark for sale'}>
                {disc.forSale ? '❌' : '💰'}
            </button>
            <button class="edit-btn" on:click={handleEdit} title="Edit disc">✏️</button>
            <button class="delete-btn" on:click={handleDelete} title="Delete disc">🗑️</button>
        </div>
    </div>
</div>

