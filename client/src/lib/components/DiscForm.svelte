<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    import { apiService, type Disc, DiscCategory } from '../api.js';

    export let disc: Partial<Disc> | null = null;
    export let isVisible = false;

    const dispatch = createEventDispatcher<{
        save: Omit<Disc, 'id' | 'createdDate' | 'flightNumbers'>;
        cancel: void;
    }>();

    let formData = {
        name: '',
        category: DiscCategory.Putter,
        brand: '',
        description: '',
        speed: 0,
        glide: 0,
        turn: 0,
        fade: 0,
        plastic: '',
        color: '',
        weight: 175,
        imagePath: ''
    };

    $: if (disc && isVisible) {
        formData = {
            name: disc.name || '',
            category: disc.category || DiscCategory.Putter,
            brand: disc.brand || '',
            description: disc.description || '',
            speed: disc.speed || 0,
            glide: disc.glide || 0,
            turn: disc.turn || 0,
            fade: disc.fade || 0,
            plastic: disc.plastic || '',
            color: disc.color || '',
            weight: disc.weight || 175,
            imagePath: disc.imagePath || ''
        };
    } else if (!disc && isVisible) {
        // Reset form for new disc
        formData = {
            name: '',
            category: DiscCategory.Putter,
            brand: '',
            description: '',
            speed: 0,
            glide: 0,
            turn: 0,
            fade: 0,
            plastic: '',
            color: '',
            weight: 175,
            imagePath: ''
        };
    }

    function handleSubmit() {
        if (!formData.name.trim()) {
            alert('Please enter a disc name');
            return;
        }

        dispatch('save', formData);
    }

    function handleCancel() {
        dispatch('cancel');
    }

    function handleKeydown(event: KeyboardEvent) {
        if (event.key === 'Escape') {
            handleCancel();
        }
    }
</script>

<!-- svelte-ignore a11y-click-events-have-key-events -->
<!-- svelte-ignore a11y-no-static-element-interactions -->
{#if isVisible}
<div class="modal-backdrop" on:click={handleCancel}>
    <div class="modal" on:click|stopPropagation on:keydown={handleKeydown}>
        <div class="modal-header">
            <h2>{disc ? 'Edit Disc' : 'Add New Disc'}</h2>
            <button class="close-btn" on:click={handleCancel}>&times;</button>
        </div>
        
        <form on:submit|preventDefault={handleSubmit} class="disc-form">
            <div class="form-group">
                <label for="name">Name *</label>
                <input 
                    type="text" 
                    id="name" 
                    bind:value={formData.name} 
                    required 
                    placeholder="e.g. Champion Roc3"
                />
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label for="brand">Brand</label>
                    <input 
                        type="text" 
                        id="brand" 
                        bind:value={formData.brand} 
                        placeholder="e.g. Innova"
                    />
                </div>

                <div class="form-group">
                    <label for="category">Category</label>
                    <select id="category" bind:value={formData.category}>
                        <option value={DiscCategory.Putter}>Putter</option>
                        <option value={DiscCategory.Mid}>Mid</option>
                        <option value={DiscCategory.Fairway}>Fairway</option>
                        <option value={DiscCategory.Driver}>Driver</option>
                    </select>
                </div>
            </div>

            <div class="form-group">
                <label for="description">Description</label>
                <textarea 
                    id="description" 
                    bind:value={formData.description} 
                    rows="3"
                    placeholder="Describe your disc..."
                ></textarea>
            </div>

            <div class="flight-numbers">
                <h4>Flight Numbers</h4>
                <div class="form-row">
                    <div class="form-group">
                        <label for="speed">Speed (1-15)</label>
                        <input 
                            type="number" 
                            id="speed" 
                            bind:value={formData.speed} 
                            min="1" 
                            max="15"
                            step="0.5"
                            placeholder="e.g. 5 or 5.5"
                        />
                    </div>
                    <div class="form-group">
                        <label for="glide">Glide (1-7)</label>
                        <input 
                            type="number" 
                            id="glide" 
                            bind:value={formData.glide} 
                            min="1" 
                            max="7"
                            step="0.5"
                            placeholder="e.g. 4 or 4.5"
                        />
                    </div>
                    <div class="form-group">
                        <label for="turn">Turn (-5 to 1)</label>
                        <input 
                            type="number" 
                            id="turn" 
                            bind:value={formData.turn} 
                            min="-5" 
                            max="1"
                            step="0.5"
                            placeholder="e.g. -1 or -0.5"
                        />
                    </div>
                    <div class="form-group">
                        <label for="fade">Fade (0-5)</label>
                        <input 
                            type="number" 
                            id="fade" 
                            bind:value={formData.fade} 
                            min="0" 
                            max="5"
                            step="0.5"
                            placeholder="e.g. 1 or 1.5"
                        />
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label for="plastic">Plastic</label>
                    <input 
                        type="text" 
                        id="plastic" 
                        bind:value={formData.plastic} 
                        placeholder="e.g. Champion"
                    />
                </div>
                <div class="form-group">
                    <label for="color">Color</label>
                    <input 
                        type="text" 
                        id="color" 
                        bind:value={formData.color} 
                        placeholder="e.g. Blue"
                    />
                </div>
                <div class="form-group">
                    <label for="weight">Weight (g)</label>
                    <input 
                        type="number" 
                        id="weight" 
                        bind:value={formData.weight} 
                        min="120" 
                        max="180"
                        placeholder="175"
                    />
                </div>
            </div>

            <div class="form-actions">
                <button type="button" class="cancel-btn" on:click={handleCancel}>
                    Cancel
                </button>
                <button type="submit" class="save-btn">
                    {disc ? 'Update' : 'Add'} Disc
                </button>
            </div>
        </form>
    </div>
</div>
{/if}

