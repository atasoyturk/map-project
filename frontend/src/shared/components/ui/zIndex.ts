// CSS stacking order for the whole app. Use via `style={{ zIndex: Z.x }}`.
// OpenLayers layer zIndex (map layer order) is separate and not listed here.
export const Z = {
  map:                 0,
  mapControl:          10,
  adminSidebar:        100,
  mapTooltip:          900,
  sidePanel:           950,
  searchBar:           960,
  toolbar:             1000,
  dropdown:            1001,
  analysisPanel:       1100,
  mapPopup:            1500,
  contextMenuBackdrop: 1900,
  contextMenu:         1901,
  modal:               2000,
  modalRaised:         3000,
  fullscreen:          4000,
  fullscreenConfirm:   5000,
} as const;
