"use strict";

class TagifySelect {
    constructor() {
        this.whitelist = [];
        this.control = null;
        this.onselect = null;
        this.optionSize = 'normal';
    }
    init() {
        const a = (e) => {
            const instance = this;
            var t;
            this.control = new Tagify(e, {
                    tagTextProp: "name",
                    enforceWhitelist: !0,
                    skipInvalid: !0,
                    dropdown: { closeOnSelect: true, enabled: 0, classname: "users-list", searchKeys: ["name", "email"] },
                    templates: {
                        tag: function (e) {
                            if (instance.optionSize == 'small') {
                                return `<tag title="${e.title || e.email}" contenteditable='false' spellcheck='false' tabIndex="-1" class="${this.settings.classNames.tag} ${e.class ? e.class : ""}"
                                        ${this.getAttributes(e)}>              
                                        <x title='' class='tagify__tag__removeBtn' role='button' aria-label='remove tag'></x>\n                    
                                        <div class="d-flex align-items-center">
                                            <span>${e.name}</span>
                                        </div>
                                    </tag>`;

                            }
                            else {
                                return `<tag title="${e.title || e.email}" contenteditable='false' spellcheck='false' tabIndex="-1" class="${this.settings.classNames.tag} ${e.class ? e.class : ""}"
                                        ${this.getAttributes(e)}>              
                                        <x title='' class='tagify__tag__removeBtn' role='button' aria-label='remove tag'></x>\n                    
                                        <div class="d-flex align-items-center">
                                            <div class='tagify__tag__avatar-wrap ps-0'>
                                                <div class="symbol symbol-circle symbol-32px">
                                                    ${e.icon}
                                                </div>
                                            </div>
                                            <span class='tagify__tag-text'>${e.name}</span>                   
                                        </div>
                                    </tag>`;
                            }
                            
                        },
                        dropdownItem: function (e) {
                            return `<div ${this.getAttributes(e)} class='tagify__dropdown__item d-flex align-items-center ${e.class ? e.class : ""}' tabindex="0" role="option">
                                        ${e.icon ? `
                                        <div class='tagify__dropdown__item__avatar-wrap me-2'>
                                            <div class="symbol symbol-circle symbol-50px me-5">
                                                ${e.icon}
                                            </div>
                                        </div>` : ""}
                                        <div class="d-flex flex-column">
                                            <strong>${e.name}</strong>
                                            <span>${e.email}</span>
                                        </div>
                                   </div>`;
                        },
                    },
                    whitelist: this.whitelist,
                });
            this.control.on("dropdown:select", function (e) {
                e.detail.elm == t && this.control.dropdown.selectAll.call(a);
                if (instance.onselect) instance.onselect(e.detail.data);
            });
            this.control.on("remove", function (e) {
                if (instance.onremove) instance.onremove(e.detail.data);
            });
        }
        const l = document.querySelectorAll('[data-tagify-select="true"]');
        l.forEach((e) => {
            a(e);
        })
    }
}



