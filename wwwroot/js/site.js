// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function getBrowserCulture() {
    return (navigator.language || document.documentElement.lang || 'en').toLowerCase();
}

function isCroatianCulture(cultureCode) {
    return (cultureCode || '').toLowerCase().startsWith('hr');
}

function formatRosterDate(dateValue, cultureCode, mode) {
    if (!(dateValue instanceof Date) || isNaN(dateValue.getTime())) {
        return '';
    }

    const pad = (value) => String(value).padStart(2, '0');
    const day = pad(dateValue.getDate());
    const month = pad(dateValue.getMonth() + 1);
    const year = dateValue.getFullYear();
    const hours = pad(dateValue.getHours());
    const minutes = pad(dateValue.getMinutes());
    const isDateOnly = (mode || '').toLowerCase() === 'date';

    if ((cultureCode || '').toLowerCase().startsWith('hr')) {
        return isDateOnly ? `${day}.${month}.${year}` : `${day}.${month}.${year} ${hours}:${minutes}`;
    }

    return isDateOnly ? `${month}/${day}/${year}` : `${month}/${day}/${year} ${hours}:${minutes}`;
}

function parseRosterDate(value, cultureCode, mode) {
    if (!value) {
        return null;
    }

    const trimmed = value.trim();
    const isDateOnly = (mode || '').toLowerCase() === 'date';
    const hrDateOnlyMatch = trimmed.match(/^(\d{2})\.(\d{2})\.(\d{4})$/);
    const hrDateTimeMatch = trimmed.match(/^(\d{2})\.(\d{2})\.(\d{4})\s+(\d{2}):(\d{2})$/);

    if ((cultureCode || '').toLowerCase().startsWith('hr')) {
        if (isDateOnly && hrDateOnlyMatch) {
            const [, day, month, year] = hrDateOnlyMatch;
            return new Date(Number(year), Number(month) - 1, Number(day), 0, 0, 0, 0);
        }

        if (!isDateOnly && hrDateTimeMatch) {
            const [, day, month, year, hours, minutes] = hrDateTimeMatch;
            return new Date(Number(year), Number(month) - 1, Number(day), Number(hours), Number(minutes), 0, 0);
        }
    }

    const enDateOnlyMatch = trimmed.match(/^(\d{2})\/(\d{2})\/(\d{4})$/);
    const enDateTimeMatch = trimmed.match(/^(\d{2})\/(\d{2})\/(\d{4})\s+(\d{2}):(\d{2})$/);

    if (isDateOnly && enDateOnlyMatch) {
        const [, month, day, year] = enDateOnlyMatch;
        return new Date(Number(year), Number(month) - 1, Number(day), 0, 0, 0, 0);
    }

    if (!isDateOnly && enDateTimeMatch) {
        const [, month, day, year, hours, minutes] = enDateTimeMatch;
        return new Date(Number(year), Number(month) - 1, Number(day), Number(hours), Number(minutes), 0, 0);
    }

    return null;
}

$(function () {
    if ($.validator && $.validator.setDefaults) {
        $.validator.setDefaults({ ignore: [] });
    }

    $('[data-flatpickr-picker]').each(function () {
        const $wrapper = $(this);
        const $visibleInput = $wrapper.find('[data-flatpickr-input]');
        const $hiddenInput = $wrapper.find('[data-flatpickr-hidden]');
        const mode = ($wrapper.data('flatpickrMode') || 'datetime').toString();
        const cultureCode = ($wrapper.data('flatpickrCulture') || getBrowserCulture()).toString();
        const croatian = isCroatianCulture(cultureCode);

        if (!$visibleInput.length || !$hiddenInput.length || typeof flatpickr === 'undefined') {
            return;
        }

        const toHiddenValue = (date) => {
            if (!(date instanceof Date) || isNaN(date.getTime())) {
                return '';
            }

            if (mode === 'date') {
                const year = date.getFullYear();
                const month = String(date.getMonth() + 1).padStart(2, '0');
                const day = String(date.getDate()).padStart(2, '0');
                return `${year}-${month}-${day}`;
            }

            return date.toISOString();
        };

        const parseDateOnly = (value) => {
            if (!value) {
                return null;
            }

            const parsed = flatpickr.parseDate(value, 'Y-m-d');
            return parsed instanceof Date && !isNaN(parsed.getTime()) ? parsed : null;
        };

        const parseDateTime = (value) => {
            if (!value) {
                return null;
            }

            const parsed = new Date(value);
            return parsed instanceof Date && !isNaN(parsed.getTime()) ? parsed : null;
        };

        const dateFormat = mode === 'date'
            ? (croatian ? 'd.m.Y' : 'm/d/Y')
            : (croatian ? 'd.m.Y H:i' : 'm/d/Y H:i');

        const initialHiddenValue = $hiddenInput.val();
        const initialDate = mode === 'date'
            ? parseDateOnly(initialHiddenValue)
            : parseDateTime(initialHiddenValue);

        const options = {
            allowInput: true,
            clickOpens: true,
            disableMobile: true,
            animate: true,
            time_24hr: true,
            minuteIncrement: 1,
            dateFormat: dateFormat,
            enableTime: mode !== 'date',
            defaultDate: initialDate || $visibleInput.val() || undefined,
            locale: croatian && flatpickr.l10ns && flatpickr.l10ns.hr ? flatpickr.l10ns.hr : undefined,
            onReady: function (selectedDates, dateStr, instance) {
                instance.calendarContainer?.classList.add('cs2-flatpickr-calendar');
                instance.input.setAttribute('autocomplete', 'off');
                if (!instance.input.value && initialDate) {
                    instance.setDate(initialDate, false);
                }
            },
            onChange: function (selectedDates, dateStr, instance) {
                const selected = selectedDates && selectedDates.length > 0 ? selectedDates[0] : null;
                $hiddenInput.val(selected ? toHiddenValue(selected) : '');
            },
            onValueUpdate: function (selectedDates, dateStr, instance) {
                const selected = selectedDates && selectedDates.length > 0 ? selectedDates[0] : null;
                $hiddenInput.val(selected ? toHiddenValue(selected) : '');
            }
        };

        const picker = flatpickr($visibleInput[0], options);

        $visibleInput.on('change blur', function () {
            const rawValue = $(this).val().trim();
            if (!rawValue) {
                $hiddenInput.val('');
                return;
            }

            const parsed = picker.parseDate(rawValue, dateFormat);
            if (!parsed) {
                return;
            }

            picker.setDate(parsed, false);
            $hiddenInput.val(toHiddenValue(parsed));
        });
    });

    $('[data-match-editor]').each(function () {
        const $editor = $(this);
        const $formatSelect = $editor.closest('form').find('select[name="Format"]');
        const $rows = $editor.find('[data-match-map-row]');
        const $seriesScoreA = $editor.find('[data-match-series-score-a]');
        const $seriesScoreB = $editor.find('[data-match-series-score-b]');
        const $resetButton = $editor.find('[data-match-map-reset]');

        const getFormatValue = () => parseInt($formatSelect.val(), 10) || parseInt($editor.data('matchMaxMaps'), 10) || 3;

        const syncRows = () => {
            const maxMaps = getFormatValue();
            $rows.each(function () {
                const $row = $(this);
                const rowIndex = parseInt($row.data('matchMapIndex'), 10);
                const visible = rowIndex <= maxMaps;
                $row.toggleClass('d-none', !visible);
                $row.find('select, input').prop('disabled', !visible);
            });
        };

        const syncSeriesScore = () => {
            let teamAScore = 0;
            let teamBScore = 0;

            $rows.each(function () {
                const $row = $(this);
                if ($row.hasClass('d-none')) {
                    return;
                }

                const mapSelected = ($row.find('[data-match-map-select]').val() || '').toString().trim();
                const scoreA = parseInt($row.find('[data-match-map-score-a]').val(), 10);
                const scoreB = parseInt($row.find('[data-match-map-score-b]').val(), 10);

                if (!mapSelected || Number.isNaN(scoreA) || Number.isNaN(scoreB)) {
                    return;
                }

                if (scoreA > scoreB) {
                    teamAScore += 1;
                } else if (scoreB > scoreA) {
                    teamBScore += 1;
                }
            });

            $seriesScoreA.val(teamAScore);
            $seriesScoreB.val(teamBScore);
        };

        const resetMapState = () => {
            $rows.each(function () {
                const $row = $(this);
                $row.find('[data-match-map-select]').val('');
                $row.find('[data-match-map-score-a]').val('');
                $row.find('[data-match-map-score-b]').val('');
                $row.find('[data-match-map-ot]').prop('checked', false);
            });

            syncSeriesScore();
            $editor.find('input, select').trigger('change');
        };

        $formatSelect.on('change', function () {
            syncRows();
            syncSeriesScore();
        });

        $editor.on('input change', '[data-match-map-select], [data-match-map-score-a], [data-match-map-score-b], [data-match-map-ot]', function () {
            syncSeriesScore();
        });

        $resetButton.on('click', function () {
            resetMapState();
        });

        syncRows();
        syncSeriesScore();
    });

    const minSearchLength = 2;

    $('[data-date-picker]').each(function () {
        const $wrapper = $(this);
        const cultureCode = ($wrapper.data('dateCulture') || 'en').toString();
        const mode = ($wrapper.data('dateMode') || 'datetime').toString();
        const $hidden = $wrapper.find('[data-date-picker-hidden]');
        const $input = $wrapper.find('[data-date-picker-input]');

        const syncVisible = () => {
            const rawValue = $hidden.val();
            if (!rawValue) {
                return;
            }

            if (mode === 'date') {
                const dateOnlyMatch = rawValue.match(/^(\d{4})-(\d{2})-(\d{2})/);
                if (dateOnlyMatch) {
                    const [, year, month, day] = dateOnlyMatch;
                    const parsed = new Date(Number(year), Number(month) - 1, Number(day), 0, 0, 0, 0);
                    $input.val(formatRosterDate(parsed, cultureCode, mode));
                }
                return;
            }

            const parsed = new Date(rawValue);
            if (!isNaN(parsed.getTime())) {
                $input.val(formatRosterDate(parsed, cultureCode, mode));
            }
        };

        $input.on('blur', function () {
            const parsed = parseRosterDate($(this).val(), cultureCode, mode);
            if (!parsed) {
                return;
            }

            if (mode === 'date') {
                const year = parsed.getFullYear();
                const month = String(parsed.getMonth() + 1).padStart(2, '0');
                const day = String(parsed.getDate()).padStart(2, '0');
                $hidden.val(`${year}-${month}-${day}`);
            } else {
                $hidden.val(parsed.toISOString());
            }

            $(this).val(formatRosterDate(parsed, cultureCode, mode));
        });

        $input.on('change', function () {
            const parsed = parseRosterDate($(this).val(), cultureCode, mode);
            if (!parsed) {
                return;
            }

            if (mode === 'date') {
                const year = parsed.getFullYear();
                const month = String(parsed.getMonth() + 1).padStart(2, '0');
                const day = String(parsed.getDate()).padStart(2, '0');
                $hidden.val(`${year}-${month}-${day}`);
            } else {
                $hidden.val(parsed.toISOString());
            }
        });

        syncVisible();
    });

    $('[data-player-picker]').each(function () {
        const $picker = $(this);
        const searchUrl = $picker.data('searchUrl');
        const currentTeamId = $picker.data('currentTeamId');
        const maxPlayers = parseInt($picker.data('maxPlayers'), 10) || 5;
        const $selected = $picker.find('[data-player-selected]');
        const $results = $picker.find('[data-player-results]');
        const $input = $picker.find('[data-player-search-input]');
        const $spinner = $picker.find('[data-player-spinner]');
        const $status = $picker.find('[data-player-status]');
        const $empty = $picker.find('[data-player-empty]');

        let debounceHandle = null;
        let activeRequest = null;

        const spawnFloatingNotice = (message, event, variant) => {
            const x = event?.clientX ?? ($picker.offset()?.left ?? 0) + ($picker.outerWidth() ?? 0) / 2;
            const y = event?.clientY ?? ($picker.offset()?.top ?? 0) + 40;
            const notice = $(
                `<div class="team-player-floating-notice team-player-floating-notice--${variant || 'add'}" style="left:${x}px; top:${y}px;">${message}</div>`
            );

            $('body').append(notice);
            window.setTimeout(function () {
                notice.addClass('team-player-floating-notice--fade');
            }, 950);
            window.setTimeout(function () {
                notice.remove();
            }, 1500);
        };

        const getSelectedIds = () => {
            return $selected.find('[data-player-chip]').map(function () {
                return parseInt($(this).data('playerId'), 10);
            }).get();
        };

        const updateCount = () => {
            const selectedCount = $selected.find('[data-player-chip]').length;
            const isFull = selectedCount >= maxPlayers;

            $input.prop('disabled', isFull);
            $picker.toggleClass('team-player-picker--full', isFull);

            if (selectedCount === 0) {
                $empty.removeClass('d-none');
            } else {
                $empty.addClass('d-none');
            }

            if (isFull) {
                $status.text(`Maximum ${maxPlayers} players selected.`);
            } else {
                $status.text('Start typing to find players.');
            }
        };

        const closeResults = () => {
            $results.addClass('d-none').empty();
        };

        const showSpinner = () => {
            $spinner.removeClass('d-none');
        };

        const hideSpinner = () => {
            $spinner.addClass('d-none');
        };

        const addPlayer = (playerId, playerText, event) => {
            const selectedIds = getSelectedIds();
            if (selectedIds.includes(playerId)) {
                $status.text(`${playerText} is already selected.`);
                return;
            }

            if (selectedIds.length >= maxPlayers) {
                $status.text(`You can select at most ${maxPlayers} players.`);
                return;
            }

            const chip = `
                <div class="team-player-roster-card team-player-roster-card--enter" data-player-card data-player-chip data-player-id="${playerId}" data-player-text="${playerText}">
                    <button type="button" class="team-player-roster-card__remove" data-player-remove aria-label="Remove ${playerText}">×</button>
                    <div class="team-player-roster-card__avatar">
                        <img src="/images/default-avatar.svg" alt="${playerText}" />
                    </div>
                    <div class="team-player-roster-card__body">
                        <div class="team-player-roster-card__name text-truncate">${playerText}</div>
                    </div>
                    <input type="hidden" name="SelectedPlayerIds" value="${playerId}" />
                </div>`;

            $empty.addClass('d-none');
            $selected.append(chip);
            spawnFloatingNotice(`+ ${playerText}`, event, 'add');
            $input.val('');
            closeResults();
            updateCount();
        };

        const renderResults = (items) => {
            if (!items || !items.length) {
                $results.html('<button type="button" class="dropdown-item disabled">No players found.</button>');
                $results.removeClass('d-none');
                return;
            }

            const selectedIds = getSelectedIds();
            const markup = items.map(item => {
                const isSelected = selectedIds.includes(item.id);
                return `
                    <button type="button" class="dropdown-item team-player-search__item ${isSelected ? 'disabled' : ''}" data-player-result data-player-id="${item.id}" data-player-text="${item.text}" ${isSelected ? 'disabled' : ''}>
                        <span class="team-player-search__nickname">${item.text}</span>
                        ${isSelected ? '<span class="small text-muted ms-2">Selected</span>' : ''}
                    </button>`;
            }).join('');

            $results.html(markup).removeClass('d-none');
        };

        if ($selected.find('[data-player-chip]').length === 0) {
            $empty.removeClass('d-none');
        }

        $selected.on('click', '[data-player-remove]', function (event) {
            const $chip = $(this).closest('[data-player-chip]');
            const removedText = $chip.data('playerText');
            $chip.addClass('team-player-roster-card--exit');
            $chip.one('animationend webkitAnimationEnd', function () {
                $chip.remove();
                updateCount();
            });
            spawnFloatingNotice(`- ${removedText}`, event, 'remove');
        });

        $selected.on('mouseenter', '[data-player-chip]', function () {
            $(this).addClass('team-player-roster-card--hovered');
        });

        $selected.on('mouseleave', '[data-player-chip]', function () {
            $(this).removeClass('team-player-roster-card--hovered');
        });

        $input.on('input', function () {
            const query = $(this).val().trim();

            if (activeRequest) {
                activeRequest.abort();
                activeRequest = null;
            }

            clearTimeout(debounceHandle);

            if (query.length < minSearchLength || $input.prop('disabled')) {
                closeResults();
                return;
            }

            debounceHandle = window.setTimeout(function () {
                showSpinner();
                activeRequest = $.ajax({
                    url: searchUrl,
                    method: 'GET',
                    data: currentTeamId ? { query: query, currentTeamId: currentTeamId } : { query: query },
                    dataType: 'json'
                })
                    .done(function (items) {
                        renderResults(items);
                    })
                    .fail(function () {
                        $status.text('Player search failed. Try again.');
                        closeResults();
                    })
                    .always(function () {
                        hideSpinner();
                        activeRequest = null;
                    });
            }, 220);
        });

        $input.on('focus', function () {
            if (!$results.hasClass('d-none') && $results.children().length > 0) {
                $results.removeClass('d-none');
            }
        });

        $picker.on('click', '[data-player-result]', function (event) {
            const playerId = parseInt($(this).data('playerId'), 10);
            const playerText = $(this).data('playerText');
            addPlayer(playerId, playerText, event);
        });

        $(document).on('click', function (event) {
            if (!$(event.target).closest($picker).length) {
                closeResults();
            }
        });

        updateCount();
    });

    $('[data-team-picker]').each(function () {
        const $picker = $(this);
        const searchUrl = $picker.data('searchUrl');
        const layout = ($picker.data('teamLayout') || 'cards').toString();
        const maxTeams = parseInt($picker.data('maxTeams'), 10) || 16;
        const $selected = $picker.find('[data-team-selected]');
        const $results = $picker.find('[data-team-results]');
        const $input = $picker.find('[data-team-search-input]');
        const $spinner = $picker.find('[data-team-spinner]');
        const $status = $picker.find('[data-team-status]');
        const $empty = $picker.find('[data-team-empty]');

        let debounceHandle = null;
        let activeRequest = null;

        const spawnFloatingNotice = (message, event, variant) => {
            const x = event?.clientX ?? ($picker.offset()?.left ?? 0) + ($picker.outerWidth() ?? 0) / 2;
            const y = event?.clientY ?? ($picker.offset()?.top ?? 0) + 40;
            const notice = $(
                `<div class="team-player-floating-notice team-player-floating-notice--${variant || 'add'}" style="left:${x}px; top:${y}px;">${message}</div>`
            );

            $('body').append(notice);
            window.setTimeout(function () {
                notice.addClass('team-player-floating-notice--fade');
            }, 950);
            window.setTimeout(function () {
                notice.remove();
            }, 1500);
        };

        const getSelectedIds = () => {
            return $selected.find('[data-team-card]').map(function () {
                return parseInt($(this).data('teamId'), 10);
            }).get();
        };

        const updateCount = () => {
            const selectedCount = $selected.find('[data-team-card]').length;
            const isFull = selectedCount >= maxTeams;

            $input.prop('disabled', isFull);
            $picker.toggleClass('team-player-picker--full', isFull);

            if (selectedCount === 0) {
                $empty.removeClass('d-none');
            } else {
                $empty.addClass('d-none');
            }

            if (isFull) {
                $status.text(`Maximum ${maxTeams} teams selected.`);
            } else {
                $status.text('Start typing to find teams.');
            }
        };

        const closeResults = () => {
            $results.addClass('d-none').empty();
        };

        const showSpinner = () => {
            $spinner.removeClass('d-none');
        };

        const hideSpinner = () => {
            $spinner.addClass('d-none');
        };

        const addTeam = (teamId, teamText, logoPath, badgeText, event) => {
            const selectedIds = getSelectedIds();
            if (selectedIds.includes(teamId)) {
                $status.text(`${teamText} is already selected.`);
                return;
            }

            if (selectedIds.length >= maxTeams) {
                $status.text(`You can select at most ${maxTeams} teams.`);
                return;
            }

            const card = layout === 'table'
                ? `
                <tr class="team-table-picker__row team-table-picker__row--enter" data-team-card data-team-id="${teamId}" data-team-text="${teamText}">
                    <td class="ps-3">
                        <div class="d-flex align-items-center gap-2">
                            ${logoPath ? `<img src="${logoPath}" class="team-logo" alt="${teamText} logo" />` : `<span class="team-logo-fallback">${badgeText || teamText.substring(0, 4)}</span>`}
                            <strong class="text-light">${teamText}</strong>
                        </div>
                    </td>
                    <td class="text-muted">${badgeText || ''}</td>
                    <td class="text-end pe-3">
                        <button type="button" class="btn btn-outline-danger btn-sm" data-team-remove aria-label="Remove ${teamText}">Remove</button>
                        <input type="hidden" name="SelectedTeamIds" value="${teamId}" />
                    </td>
                </tr>`
                : `
                <div class="team-player-roster-card team-player-roster-card--enter" data-team-card data-team-id="${teamId}" data-team-text="${teamText}">
                    <button type="button" class="team-player-roster-card__remove" data-team-remove aria-label="Remove ${teamText}">×</button>
                    <div class="team-player-roster-card__avatar">
                        ${logoPath ? `<img src="${logoPath}" alt="${teamText} logo" />` : `<span class="team-logo-fallback">${badgeText || teamText.substring(0, 4)}</span>`}
                    </div>
                    <div class="team-player-roster-card__body">
                        <div class="team-player-roster-card__name text-truncate">${teamText}</div>
                    </div>
                    <input type="hidden" name="SelectedTeamIds" value="${teamId}" />
                </div>`;

            $empty.addClass('d-none');
            $selected.append(card);
            spawnFloatingNotice(`+ ${teamText}`, event, 'add');
            $input.val('');
            closeResults();
            updateCount();
        };

        const renderResults = (items) => {
            if (!items || !items.length) {
                $results.html('<button type="button" class="dropdown-item disabled">No teams found.</button>');
                $results.removeClass('d-none');
                return;
            }

            const selectedIds = getSelectedIds();
            const markup = items.map(item => {
                const isSelected = selectedIds.includes(item.id);
                const logoHtml = item.logoPath ? `<img src="${item.logoPath}" class="team-logo me-2" alt="${item.text} logo" />` : `<span class="team-logo-fallback me-2">${item.badgeText || item.text.substring(0, 4)}</span>`;

                return `
                    <button type="button" class="dropdown-item team-player-search__item ${isSelected ? 'disabled' : ''}" data-team-result data-team-id="${item.id}" data-team-text="${item.text}" data-team-logo-path="${item.logoPath || ''}" data-team-badge-text="${item.badgeText || ''}" ${isSelected ? 'disabled' : ''}>
                        <span class="d-inline-flex align-items-center">${logoHtml}<span class="team-player-search__nickname">${item.text}</span></span>
                        ${isSelected ? '<span class="small text-muted ms-2">Selected</span>' : ''}
                    </button>`;
            }).join('');

            $results.html(markup).removeClass('d-none');
        };

        if ($selected.find('[data-team-card]').length === 0) {
            $empty.removeClass('d-none');
        }

        $selected.on('click', '[data-team-remove]', function (event) {
            const $card = $(this).closest('[data-team-card]');
            const removedText = $card.data('teamText');
            $card.addClass('team-player-roster-card--exit');
            $card.one('animationend webkitAnimationEnd', function () {
                $card.remove();
                updateCount();
            });
            spawnFloatingNotice(`- ${removedText}`, event, 'remove');
        });

        $input.on('input', function () {
            const query = $(this).val().trim();

            if (activeRequest) {
                activeRequest.abort();
                activeRequest = null;
            }

            clearTimeout(debounceHandle);

            if (query.length < minSearchLength || $input.prop('disabled')) {
                closeResults();
                return;
            }

            debounceHandle = window.setTimeout(function () {
                showSpinner();
                activeRequest = $.ajax({
                    url: searchUrl,
                    method: 'GET',
                    data: { query: query },
                    dataType: 'json'
                })
                    .done(function (items) {
                        renderResults(items);
                    })
                    .fail(function () {
                        $status.text('Team search failed. Try again.');
                        closeResults();
                    })
                    .always(function () {
                        hideSpinner();
                        activeRequest = null;
                    });
            }, 220);
        });

        $picker.on('click', '[data-team-result]', function (event) {
            const teamId = parseInt($(this).data('teamId'), 10);
            const teamText = $(this).data('teamText');
            const logoPath = $(this).data('teamLogoPath');
            const badgeText = $(this).data('teamBadgeText');
            addTeam(teamId, teamText, logoPath, badgeText, event);
        });

        $(document).on('click', function (event) {
            if (!$(event.target).closest($picker).length) {
                closeResults();
            }
        });

        updateCount();
    });
});
