// Initialize the page when the DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    setupTaskCardEvents();
    detectAndArrangeOverlappingTasks();
});

// Function to detect and arrange overlapping tasks
function detectAndArrangeOverlappingTasks() {
    const statusSections = document.querySelectorAll('.status-section');

    statusSections.forEach(section => {
        const cards = Array.from(section.querySelectorAll('.gantt-card'));
        const rows = [];

        // Sort cards by start position
        cards.sort((a, b) => {
            return parseFloat(a.dataset.start) - parseFloat(b.dataset.start);
        });

        // Place each card in the first row where it doesn't overlap
        cards.forEach(card => {
            const start = parseFloat(card.dataset.start);
            const end = parseFloat(card.dataset.end);

            let rowIndex = 0;
            let placed = false;

            while (!placed) {
                if (!rows[rowIndex]) {
                    rows[rowIndex] = [];
                }

                const rowCards = rows[rowIndex];
                let overlaps = false;

                for (const rowCard of rowCards) {
                    const rowCardStart = parseFloat(rowCard.dataset.start);
                    const rowCardEnd = parseFloat(rowCard.dataset.end);

                    // Check if there's any overlap
                    if (!(end < rowCardStart || start > rowCardEnd)) {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps) {
                    rows[rowIndex].push(card);
                    card.dataset.row = rowIndex;
                    card.style.top = (rowIndex * 120) + 'px'; // 120px height + spacing
                    placed = true;
                } else {
                    rowIndex++;
                }
            }
        });

        // Group overlapping cards
        console.log('Starting overlap detection for', cards.length, 'cards');
        const overlapGroups = [];

        cards.forEach(card => {
            const start = parseFloat(card.dataset.start);
            const end = parseFloat(card.dataset.end);
            console.log(`Processing card ID: ${card.dataset.id}, start: ${start}, end: ${end}`);

            // Find if this card belongs to an existing group
            let addedToGroup = false;

            for (const group of overlapGroups) {
                for (const groupCard of group) {
                    const groupCardStart = parseFloat(groupCard.dataset.start);
                    const groupCardEnd = parseFloat(groupCard.dataset.end);

                    // Check if there's any overlap
                    if (!(end < groupCardStart || start > groupCardEnd)) {
                        console.log(`Card ${card.dataset.id} overlaps with card ${groupCard.dataset.id}`);
                        group.push(card);
                        addedToGroup = true;
                        break;
                    }
                }

                if (addedToGroup) break;
            }

            // If not added to any existing group, create a new group
            if (!addedToGroup) {
                console.log(`Creating new overlap group for card ${card.dataset.id}`);
                overlapGroups.push([card]);
            }
        });

        console.log('Initial overlap groups:', overlapGroups.map(g => g.map(c => c.dataset.id)));

        // Now merge groups that have common cards
        let i = 0;
        while (i < overlapGroups.length) {
            let merged = false;
            let j = i + 1;

            while (j < overlapGroups.length) {
                // Check if groups have any common cards
                const group1Cards = overlapGroups[i];
                const group2Cards = overlapGroups[j];

                const hasCommonCard = group1Cards.some(card1 =>
                    group2Cards.some(card2 => card1 === card2)
                );

                if (hasCommonCard) {
                    console.log(`Merging overlap groups ${i} and ${j}`);
                    // Merge groups
                    overlapGroups[i] = [...new Set([...group1Cards, ...group2Cards])];
                    overlapGroups.splice(j, 1);
                    merged = true;
                } else {
                    j++;
                }
            }

            if (!merged) {
                i++;
            }
        }

        console.log('Final overlap groups after merging:', overlapGroups.map(g => g.map(c => c.dataset.id)));

        // Add custom data attributes for cycling
        overlapGroups.forEach((group, groupIndex) => {
            console.log(`Setting up group ${groupIndex} with ${group.length} cards`);

            if (group.length > 1) {
                group.forEach((card, cardIndex) => {
                    card.setAttribute('data-overlap-group', groupIndex.toString());
                    card.setAttribute('data-cycle-index', cardIndex.toString());
                    console.log(`Card ${card.dataset.id} assigned to overlap group ${groupIndex}, cycle index ${cardIndex}`);

                    // Show the recycle button only for groups with multiple cards
                    const recycleBtn = card.querySelector('.recycle-btn');
                    if (recycleBtn) {
                        recycleBtn.style.display = 'flex';
                    } else {
                        console.warn(`Recycle button not found for card ${card.dataset.id}`);
                    }
                });
            } else {
                // Hide recycle button for single cards
                const card = group[0];
                console.log(`Card ${card.dataset.id} is in a group by itself, hiding recycle button`);
                const recycleBtn = card.querySelector('.recycle-btn');
                if (recycleBtn) {
                    recycleBtn.style.display = 'none';
                } else {
                    console.warn(`Recycle button not found for card ${card.dataset.id}`);
                }
            }
        });

        // Update the container height
        const container = section.querySelector('.task-container');
        container.style.height = (rows.length * 120 + 20) + 'px';
    });
}

// Function to set up event listeners for task cards
function setupTaskCardEvents() {
    console.log('Setting up event listeners');

    // Add event listeners for recycle buttons
    const recycleButtons = document.querySelectorAll('.recycle-btn');
    console.log(`Found ${recycleButtons.length} recycle buttons`);

    recycleButtons.forEach(btn => {
        console.log('Adding click event to recycle button', btn);
        btn.addEventListener('click', (e) => {
            console.log('Recycle button clicked', e.target);
            e.stopPropagation();

            // Make sure we get the button element regardless of where exactly in the button was clicked
            const recycleBtn = e.target.closest('.recycle-btn');
            console.log('Located recycle button element:', recycleBtn);

            const taskCard = recycleBtn.closest('.gantt-card');
            console.log('Found parent task card:', taskCard);

            if (taskCard) {
                cycleOverlappingCards(taskCard);
            } else {
                console.error('Could not find parent task card');
            }
        });
    });

    // Add event listeners for task cards (for selection/detail view)
    const cards = document.querySelectorAll('.gantt-card');
    console.log(`Found ${cards.length} task cards`);

    cards.forEach(card => {
        card.addEventListener('click', () => {
            const taskId = card.getAttribute('data-id');
            console.log(`Card clicked, ID: ${taskId}`);
            showTaskDetails(taskId);
        });
    });
}

// Function to cycle through overlapping cards
function cycleOverlappingCards(clickedCard) {
    console.log('Cycle function called on card:', clickedCard);
    console.log('Card dataset:', clickedCard.dataset);

    const groupIndex = clickedCard.dataset.overlapGroup;
    console.log('Group index:', groupIndex);

    if (!groupIndex) {
        console.warn('Card is not part of an overlap group. Dataset:', clickedCard.dataset);
        return; // Not part of an overlap group
    }

    const section = clickedCard.closest('.status-section');
    console.log('Section found:', section);

    const selector = `.gantt-card[data-overlap-group="${groupIndex}"]`;
    console.log('Using selector:', selector);

    const groupCards = Array.from(section.querySelectorAll(selector));
    console.log('Found group cards:', groupCards.length, groupCards);

    if (groupCards.length <= 1) {
        console.warn('Nothing to cycle - only 1 or 0 cards found');
        return; // Nothing to cycle
    }

    // Get all cards in the same group
    groupCards.sort((a, b) => {
        return parseInt(a.dataset.cycleIndex) - parseInt(b.dataset.cycleIndex);
    });
    console.log('Sorted cards by cycle index:', groupCards.map(c => c.dataset.cycleIndex));

    // Bring the last card to the top
    const lastCard = groupCards[groupCards.length - 1];
    console.log('Last card that will come to top:', lastCard);

    // Update z-index and visual properties
    groupCards.forEach((card, idx) => {
        console.log(`Processing card ${idx}, current cycle index: ${card.dataset.cycleIndex}`);

        // Reset all cards first
        card.classList.remove('top-card');
        card.style.zIndex = 2;

        // Update cycle index
        const newIndex = (parseInt(card.dataset.cycleIndex) + 1) % groupCards.length;
        card.dataset.cycleIndex = newIndex;
        console.log(`Card ${idx} new cycle index: ${newIndex}`);

        // If this is now the top card (index 0), enhance it
        if (newIndex === 0) {
            console.log(`Card ${idx} is now the top card`);
            card.classList.add('top-card');
            card.style.zIndex = 5;

            // Bring to front visually
            const cardParent = card.parentNode;
            cardParent.appendChild(card);
        }
    });

    console.log('Cycling complete');
}

// Function to show task details
function showTaskDetails(taskId) {
    // Implementation for showing task details
    console.log(`Task details requested for: ${taskId}`);
    // You can implement a detail view or navigate to a details page
}


// Simple jQuery script to show list view when first gantt-card is clicked
$(document).on('click', '.timeline-grid .gantt-card:last-child', function(e) {
    // Prevent default behavior
    e.preventDefault();
    // debugger;
    // Don't trigger when clicking on control elements
    if ($(e.target).closest('.card-controls').length) {
      return;
    }
    
    // Get the parent timeline-grid
    const $timelineGrid = $(this).closest('.timeline-grid');
    
    // Get all gantt-cards in this timeline-grid
    const $cards = $timelineGrid.find('.gantt-card');
    
    // Create and append expanded view container if it doesn't exist
    if ($('#expanded-view').length === 0) {
      $('body').append('<div id="expanded-view" class="expanded-view"><div class="expanded-header"><h3>Tasks</h3><span class="close-btn">&times;</span></div><div class="cards-list"></div></div>');
      $('body').append('<div class="overlay"></div>');
      
      // Add required CSS
      $('head').append(`
        <style>
          .expanded-view {
            display: none;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            width: 80%;
            max-width: 800px;
            max-height: 80vh;
            background-color: white;
            border-radius: 8px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
            z-index: 1000;
            overflow: hidden;
            flex-direction: column;
          }
          
          .expanded-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 15px 20px;
            background-color: #f8f9fa;
            border-bottom: 1px solid #eaeaea;
          }
          
          .expanded-header h3 {
            margin: 0;
            font-size: 18px;
            font-weight: 600;
          }
          
          .close-btn {
            font-size: 24px;
            cursor: pointer;
            color: #666;
          }
          
          .close-btn:hover {
            color: #333;
          }
          
          .cards-list {
            padding: 20px;
            overflow-y: auto;
            max-height: calc(80vh - 60px);
          }
          
          .list-card {
            margin-bottom: 15px;
            padding: 15px;
            border-radius: 6px;
            border: 1px solid #eaeaea;
            transition: all 0.2s ease;
          }
          
          .list-card:hover {
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
          }
          
          .list-card .header {
            display: flex;
            align-items: center;
            margin-bottom: 10px;
          }
          
          .list-card .icon {
            width: 32px;
            height: 32px;
            margin-right: 10px;
            border-radius: 4px;
            overflow: hidden;
          }
          
          .list-card .icon img {
            width: 100%;
            height: 100%;
            object-fit: cover;
          }
          
          .list-card .title {
            font-size: 16px;
            font-weight: 600;
            margin: 0;
          }
          
          .list-card .description {
            margin-bottom: 10px;
            color: #666;
          }
          
          .list-card .details {
            font-size: 13px;
            color: #666;
          }
          
          .overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: rgba(0, 0, 0, 0.5);
            z-index: 999;
          }
        </style>
      `);
    }
    
    // Clear previous cards
    $('.cards-list').empty();
    
    // Add cards to expanded view
    $cards.each(function() {
      const id = $(this).data('id');
      const title = $(this).find('.title').text();
      const description = $(this).find('.description').text();
      const details = $(this).find('.details').html() || '';
      const iconSrc = $(this).find('.icon img').attr('src') || '';
      
      const listCard = `
        <div class="list-card" data-id="${id}">
          <div class="header">
            <div class="icon">
              <img src="${iconSrc}" alt="Task Icon">
            </div>
            <h5 class="title">${title}</h5>
          </div>
          <p class="description">${description}</p>
          <div class="details">${details}</div>
        </div>
      `;
      
      $('.cards-list').append(listCard);
    });
    
    // Show overlay and expanded view
    $('.overlay').fadeIn(200);
    $('#expanded-view').css('display', 'flex').hide().fadeIn(200);
    
    // Handle close events
    $('.close-btn, .overlay').off('click').on('click', function() {
      $('#expanded-view').fadeOut(200);
      $('.overlay').fadeOut(200);
    });
  });