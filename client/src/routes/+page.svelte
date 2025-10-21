<script lang="ts">
    import { onMount } from 'svelte';
    import { slide } from 'svelte/transition';
    import { cubicOut } from 'svelte/easing';
    import { apiService, type Disc, DiscCategory } from '$lib/api.js';
    import { localStorageManager } from '$lib/stores.js';
    import DiscCard from '$lib/components/DiscCard.svelte';
    import DiscForm from '$lib/components/DiscForm.svelte';

    let discs: Disc[] = [];
    let filteredDiscs: Disc[] = [];
    let selectedCategory: DiscCategory | 'all' = 'all';
    let bagFilter: 'all' | 'in-bag' | 'not-in-bag' = 'all';
    let saleFilter: 'all' | 'for-sale' | 'not-for-sale' = 'all';
    let favoriteFilter: 'all' | 'favorites' | 'not-favorites' = 'all';
    let searchQuery = '';
    let loading = true;
    let error = '';

    // Form state
    let showForm = false;
    let editingDisc: Disc | null = null;

    // Image upload
    let imageUploadDiscId: number | null = null;
    let imageInput: HTMLInputElement;

    // Dropdown states
    let showCategoryDropdown = false;
    let showBagDropdown = false;
    let showSaleDropdown = false;
    let showFavoriteDropdown = false;

    onMount(() => {
        loadDiscs();
    });
    
    
    async function loadDiscs() {
        try {
            loading = true;
            error = '';
            const apiDiscs = await apiService.getAllDiscs();
            
            // Add local bag, sale, and favorite status to each disc
            discs = apiDiscs.map(disc => ({
                ...disc,
                inTheBag: localStorageManager.isInBag(disc.id),
                forSale: localStorageManager.isForSale(disc.id),
                isFavorite: localStorageManager.isFavorite(disc.id)
            }));
            
            filterDiscs();
        } catch (err) {
            error = err instanceof Error ? err.message : 'Failed to load discs';
        } finally {
            loading = false;
        }
    }

    function filterDiscs() {
        let result = discs;

        // Filter by category
        if (selectedCategory !== 'all') {
            result = result.filter(disc => disc.category === selectedCategory);
        }

        // Filter by bag status
        if (bagFilter === 'in-bag') {
            result = result.filter(disc => disc.inTheBag);
        } else if (bagFilter === 'not-in-bag') {
            result = result.filter(disc => !disc.inTheBag);
        }

        // Filter by sale status
        if (saleFilter === 'for-sale') {
            result = result.filter(disc => disc.forSale);
        } else if (saleFilter === 'not-for-sale') {
            result = result.filter(disc => !disc.forSale);
        }

        // Filter by favorite status
        if (favoriteFilter === 'favorites') {
            result = result.filter(disc => disc.isFavorite);
        } else if (favoriteFilter === 'not-favorites') {
            result = result.filter(disc => !disc.isFavorite);
        }

        // Filter by search query
        if (searchQuery.trim()) {
            const query = searchQuery.toLowerCase();
            result = result.filter(disc => 
                disc.name.toLowerCase().includes(query) ||
                disc.brand.toLowerCase().includes(query) ||
                disc.description.toLowerCase().includes(query)
            );
        }

        filteredDiscs = result;
    }

    function handleAddDisc() {
        editingDisc = null;
        showForm = true;
    }

    function handleEditDisc(event: CustomEvent<Disc>) {
        editingDisc = event.detail;
        showForm = true;
    }

    async function handleDeleteDisc(event: CustomEvent<number>) {
        try {
            await apiService.deleteDisc(event.detail);
            await loadDiscs();
        } catch (err) {
            alert('Failed to delete disc: ' + (err instanceof Error ? err.message : 'Unknown error'));
        }
    }

    function handleImageUpload(event: CustomEvent<number>) {
        imageUploadDiscId = event.detail;
        imageInput.click();
    }

    async function handleImageSelected(event: Event) {
        const input = event.target as HTMLInputElement;
        const file = input.files?.[0];
        
        if (!file || !imageUploadDiscId) return;

        try {
            await apiService.uploadDiscImage(imageUploadDiscId, file);
            await loadDiscs();
            imageUploadDiscId = null;
        } catch (err) {
            alert('Failed to upload image: ' + (err instanceof Error ? err.message : 'Unknown error'));
        }
    }

    function handleToggleBag(event: CustomEvent<number>) {
        const discId = event.detail;
        const disc = discs.find(d => d.id === discId);
        
        if (disc) {
            if (disc.inTheBag) {
                localStorageManager.removeFromBag(discId);
                disc.inTheBag = false;
            } else {
                localStorageManager.addToBag(discId);
                disc.inTheBag = true;
            }
            
            // Update the discs array to trigger reactivity
            discs = [...discs];
            filterDiscs();
        }
    }

    function handleToggleForSale(event: CustomEvent<number>) {
        const discId = event.detail;
        const disc = discs.find(d => d.id === discId);
        
        if (disc) {
            if (disc.forSale) {
                localStorageManager.removeFromForSale(discId);
                disc.forSale = false;
            } else {
                localStorageManager.addToForSale(discId);
                disc.forSale = true;
            }
            
            // Update the discs array to trigger reactivity
            discs = [...discs];
            filterDiscs();
        }
    }

    function handleToggleFavorite(event: CustomEvent<number>) {
        const discId = event.detail;
        const disc = discs.find(d => d.id === discId);
        
        if (disc) {
            if (disc.isFavorite) {
                localStorageManager.removeFromFavorites(discId);
                disc.isFavorite = false;
            } else {
                localStorageManager.addToFavorites(discId);
                disc.isFavorite = true;
            }
            
            // Update the discs array to trigger reactivity
            discs = [...discs];
            filterDiscs();
        }
    }

    async function handleSaveDisc(event: CustomEvent) {
        try {
            const discData = event.detail;
            
            if (editingDisc) {
                // Clean the disc data to remove local properties before updating
                const cleanDiscData = {
                    id: editingDisc.id,
                    name: discData.name,
                    category: discData.category,
                    brand: discData.brand,
                    description: discData.description,
                    speed: discData.speed,
                    glide: discData.glide,
                    turn: discData.turn,
                    fade: discData.fade,
                    plastic: discData.plastic,
                    color: discData.color,
                    weight: discData.weight,
                    imagePath: editingDisc.imagePath || '',
                    createdDate: editingDisc.createdDate,
                    flightNumbers: editingDisc.flightNumbers
                };
                await apiService.updateDisc(editingDisc.id, cleanDiscData);
            } else {
                // For new discs, just use the form data (no local properties)
                await apiService.createDisc(discData);
            }
            await loadDiscs();
            showForm = false;
            editingDisc = null;
        } catch (err) {
            alert('Failed to save disc: ' + (err instanceof Error ? err.message : 'Unknown error'));
        }
    }

    function handleCancelForm() {
        showForm = false;
        editingDisc = null;
    }

    // $: {
    //     selectedCategory;
    //     bagFilter;
    //     saleFilter;
    //     favoriteFilter;
    //     searchQuery;
    //     filterDiscs();
    // }

    $: bagCount = discs.filter(disc => disc.inTheBag).length;
    $: saleCount = discs.filter(disc => disc.forSale).length;
    $: favoriteCount = discs.filter(disc => disc.isFavorite).length;

    // Dropdown options
    const categoryOptions = [
        { value: 'all' as const, label: 'All Categories' },
        { value: DiscCategory.Putter, label: 'Putter' },
        { value: DiscCategory.Mid, label: 'Mid' },
        { value: DiscCategory.Fairway, label: 'Fairway' },
        { value: DiscCategory.Driver, label: 'Driver' }
    ];

    const bagOptions = [
        { value: 'all' as const, label: 'All Discs' },
        { value: 'in-bag' as const, label: 'In the Bag' },
        { value: 'not-in-bag' as const, label: 'Not in Bag' }
    ];

    const saleOptions = [
        { value: 'all' as const, label: 'All Discs' },
        { value: 'for-sale' as const, label: 'For Sale' },
        { value: 'not-for-sale' as const, label: 'Not For Sale' }
    ];

    const favoriteOptions = [
        { value: 'all' as const, label: 'All Discs' },
        { value: 'favorites' as const, label: 'Favorites Only' },
        { value: 'not-favorites' as const, label: 'Not Favorites' }
    ];

    // Helper functions to get selected labels
    $: selectedCategoryLabel = categoryOptions.find(opt => opt.value === selectedCategory)?.label || 'All Categories';
    $: selectedBagLabel = bagOptions.find(opt => opt.value === bagFilter)?.label || 'All Discs';
    $: selectedSaleLabel = saleOptions.find(opt => opt.value === saleFilter)?.label || 'All Discs';
    $: selectedFavoriteLabel = favoriteOptions.find(opt => opt.value === favoriteFilter)?.label || 'All Discs';

    // Close dropdowns when clicking outside
    function handleClickOutside(event: MouseEvent) {
        const target = event.target as HTMLElement;
        if (!target.closest('.custom-dropdown')) {
            showCategoryDropdown = false;
            showBagDropdown = false;
            showSaleDropdown = false;
            showFavoriteDropdown = false;
        }
    }
</script>

<svelte:window on:click={handleClickOutside} />

<main>
    <header>
        <h1>Williams Disc golf Collection</h1>
        <div class="header-actions">
            <div class="bag-status">
                <span class="bag-count">🎒 {bagCount} disc{bagCount !== 1 ? 's' : ''} in bag</span>
            </div>
            <div class="sale-status">
                <span class="sale-count">💰 {saleCount} disc{saleCount !== 1 ? 's' : ''} for sale</span>
            </div>
            <div class="favorite-status">
                <span class="favorite-count">❤️ {favoriteCount} favorite{favoriteCount !== 1 ? 's' : ''}</span>
            </div>
            <button class="add-btn" on:click={handleAddDisc}>+ Add Disc</button>
        </div>
    </header>

    <div class="filters">
        <div class="search">
            <input 
                type="text" 
                placeholder="Search discs..." 
                bind:value={searchQuery}
            />
        </div>
        
        <div class="filter-group">
            <div class="category-filter custom-dropdown">
                <div class="filter-label">Category:</div>
                <!-- svelte-ignore a11y-click-events-have-key-events -->
                <!-- svelte-ignore a11y-no-static-element-interactions -->
                <div class="dropdown-trigger" on:click={() => showCategoryDropdown = !showCategoryDropdown}>
                    <span>{selectedCategoryLabel}</span>
                    <span class="arrow" class:rotated={showCategoryDropdown}>▼</span>
                </div>
                {#if showCategoryDropdown}
                    <div class="dropdown-menu" transition:slide={{ duration: 250, easing: cubicOut }}>
                        {#each categoryOptions as option}
                            <!-- svelte-ignore a11y-click-events-have-key-events -->
                            <!-- svelte-ignore a11y-no-static-element-interactions -->
                            <div 
                                class="dropdown-option" 
                                class:selected={option.value === selectedCategory}
                                on:click={() => {
                                    selectedCategory = option.value;
                                    showCategoryDropdown = false;
                                }}
                            >
                                {option.label}
                            </div>
                        {/each}
                    </div>
                {/if}
            </div>
            
            <div class="bag-filter custom-dropdown">
                <div class="filter-label">Bag Status:</div>
                <!-- svelte-ignore a11y-click-events-have-key-events -->
                <!-- svelte-ignore a11y-no-static-element-interactions -->
                <div class="dropdown-trigger" on:click={() => showBagDropdown = !showBagDropdown}>
                    <span>{selectedBagLabel}</span>
                    <span class="arrow" class:rotated={showBagDropdown}>▼</span>
                </div>
                {#if showBagDropdown}
                    <div class="dropdown-menu" transition:slide={{ duration: 250, easing: cubicOut }}>
                        {#each bagOptions as option}
                            <!-- svelte-ignore a11y-click-events-have-key-events -->
                            <!-- svelte-ignore a11y-no-static-element-interactions -->
                            <div 
                                class="dropdown-option" 
                                class:selected={option.value === bagFilter} 
                                on:click={() => {
                                    bagFilter = option.value;
                                    showBagDropdown = false;
                                }}
                            >
                                {option.label}
                            </div>
                        {/each}
                    </div>
                {/if}
            </div>
            
            <div class="sale-filter custom-dropdown">
                <div class="filter-label">Sale Status:</div>
                <!-- svelte-ignore a11y-click-events-have-key-events -->
                <!-- svelte-ignore a11y-no-static-element-interactions -->
                <div class="dropdown-trigger" on:click={() => showSaleDropdown = !showSaleDropdown}>
                    <span>{selectedSaleLabel}</span>
                    <span class="arrow" class:rotated={showSaleDropdown}>▼</span>
                </div>
                {#if showSaleDropdown}
                    <div class="dropdown-menu" transition:slide={{ duration: 250, easing: cubicOut }}>
                        {#each saleOptions as option}
                            <!-- svelte-ignore a11y-click-events-have-key-events -->
                            <!-- svelte-ignore a11y-no-static-element-interactions -->
                            <div 
                                class="dropdown-option" 
                                class:selected={option.value === saleFilter}
                                on:click={() => {
                                    saleFilter = option.value;
                                    showSaleDropdown = false;
                                }}
                            >
                                {option.label}
                            </div>
                        {/each}
                    </div>
                {/if}
            </div>
            
            <div class="favorite-filter custom-dropdown">
                <div class="filter-label">Favorites:</div>
                <!-- svelte-ignore a11y-click-events-have-key-events -->
                <!-- svelte-ignore a11y-no-static-element-interactions -->
                <div class="dropdown-trigger" on:click={() => showFavoriteDropdown = !showFavoriteDropdown}>
                    <span>{selectedFavoriteLabel}</span>
                    <span class="arrow" class:rotated={showFavoriteDropdown}>▼</span>
                </div>
                {#if showFavoriteDropdown}
                    <div class="dropdown-menu" transition:slide={{ duration: 250, easing: cubicOut }}>
                        {#each favoriteOptions as option}
                            <!-- svelte-ignore a11y-click-events-have-key-events -->
                            <!-- svelte-ignore a11y-no-static-element-interactions -->
                            <div 
                                class="dropdown-option" 
                                class:selected={option.value === favoriteFilter}
                                on:click={() => {
                                    favoriteFilter = option.value;
                                    showFavoriteDropdown = false;
                                }}
                            >
                                {option.label}
                            </div>
                        {/each}
                    </div>
                {/if}
            </div>
        </div>
    </div>

    {#if loading}
        <div class="loading">Loading your disc collection...</div>
    {:else if error}
        <div class="error">
            <p>Error: {error}</p>
            <button on:click={loadDiscs}>Try Again</button>
        </div>
    {:else if filteredDiscs.length === 0}
        <div class="empty-state">
            {#if discs.length === 0}
                <p>No discs in your collection yet.</p>
                <button on:click={handleAddDisc}>Add Your First Disc</button>
            {:else}
                <p>No discs match your search criteria.</p>
            {/if}
        </div>
    {:else}
        <div class="disc-grid">
            {#each filteredDiscs as disc (disc.id)}
                <DiscCard 
                    {disc} 
                    on:edit={handleEditDisc}
                    on:delete={handleDeleteDisc}
                    on:imageUpload={handleImageUpload}
                    on:toggleBag={handleToggleBag}
                    on:toggleForSale={handleToggleForSale}
                    on:toggleFavorite={handleToggleFavorite}
                />
            {/each}
        </div>
    {/if}

    <div class="stats">
        <p>{discs.length} disc{discs.length !== 1 ? 's' : ''} in collection</p>
        {#if selectedCategory !== 'all' || bagFilter !== 'all' || saleFilter !== 'all' || favoriteFilter !== 'all' || searchQuery}
            <p>({filteredDiscs.length} shown)</p>
        {/if}
        {#if bagCount > 0}
            <p class="bag-stats">🎒 {bagCount} in your bag</p>
        {/if}
        {#if saleCount > 0}
            <p class="sale-stats">💰 {saleCount} for sale</p>
        {/if}
        {#if favoriteCount > 0}
            <p class="favorite-stats">❤️ {favoriteCount} favorite{favoriteCount !== 1 ? 's' : ''}</p>
        {/if}
    </div>
</main>

<!-- Form Modal -->
<DiscForm 
    disc={editingDisc}
    isVisible={showForm}
    on:save={handleSaveDisc}
    on:cancel={handleCancelForm}
/>

<!-- Hidden file input for image uploads -->
<input 
    bind:this={imageInput}
    type="file" 
    accept="image/*" 
    style="display: none" 
    on:change={handleImageSelected}
/>

<style>
    .filter-label {
        font-weight: 600;
        margin-bottom: 0.5rem;
        color: var(--text-primary);
        font-size: 0.875rem;
    }

    .custom-dropdown {
        position: relative;
        min-width: 180px;
    }

    .dropdown-trigger {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 0.75rem 1rem;
        background: var(--bg-card);
        border: 2px solid var(--border-color);
        border-radius: 8px;
        cursor: pointer;
        font-size: 1rem;
        font-weight: 500;
        color: var(--text-primary);
        transition: all 0.2s ease;
        box-shadow: 0 2px 4px var(--shadow);
        width: 100%;
        height: 50px;
        box-sizing: border-box;
    }

    .dropdown-trigger:hover {
        border-color: var(--accent-secondary);
        box-shadow: 0 2px 8px var(--shadow-hover);
        transform: translateY(-1px);
    }

    .arrow {
        transition: transform 0.3s ease;
        font-size: 0.8rem;
        margin-left: 0.75rem;
        color: var(--text-secondary);
        font-weight: bold;
    }

    .arrow.rotated {
        transform: rotate(180deg);
        color: var(--accent-secondary);
    }

    .dropdown-menu {
        position: absolute;
        top: calc(100% + 4px);
        left: 0;
        right: 0;
        background: var(--bg-card);
        border: 2px solid var(--border-color);
        border-radius: 8px;
        box-shadow: 0 8px 24px var(--shadow-hover);
        z-index: 100;
        max-height: 240px;
        overflow-y: auto;
        backdrop-filter: blur(10px);
    }

    .dropdown-option {
        padding: 0.75rem 1rem;
        cursor: pointer;
        font-size: 1rem;
        font-weight: 500;
        color: var(--text-primary);
        transition: all 0.15s ease;
        border-radius: 6px;
        margin: 2px;
        position: relative;
    }

    .dropdown-option:hover {
        background-color: var(--bg-secondary);
        transform: translateX(2px);
    }

    .dropdown-option.selected {
        background: linear-gradient(135deg, var(--accent-secondary), var(--accent-secondary-hover));
        font-weight: 600;
        color: white;
        box-shadow: 0 2px 4px var(--shadow);
    }

    .dropdown-option.selected:hover {
        background: linear-gradient(135deg, var(--accent-secondary-hover), var(--accent-secondary));
        transform: translateX(2px);
    }

    .dropdown-option.selected::before {
        content: '✓';
        position: absolute;
        left: 0.5rem;
        font-weight: bold;
    }

    .dropdown-option.selected {
        padding-left: 2rem;
    }
</style>

