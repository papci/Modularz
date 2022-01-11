var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

import React from "react";
import ReactDOM from 'react-dom';
import { Editor } from '@tinymce/tinymce-react';

var PostEditor = function (_React$Component) {
    _inherits(PostEditor, _React$Component);

    function PostEditor(props) {
        _classCallCheck(this, PostEditor);

        var _this = _possibleConstructorReturn(this, (PostEditor.__proto__ || Object.getPrototypeOf(PostEditor)).call(this, props));

        console.log(props);
        _this.assignState(props);
        _this.handleChange = _this.handleChange.bind(_this);
        _this.handleSubmit = _this.handleSubmit.bind(_this);
        return _this;
    }

    _createClass(PostEditor, [{
        key: 'handleChange',
        value: function handleChange(event) {
            var name = event.target.name;
            var value = event.target.value;
            console.log(name, value);
            this.setState(_defineProperty({}, name, value));
        }
    }, {
        key: 'handleSubmit',
        value: function handleSubmit(event) {
            console.log(this.state);
            var uri = this.state.id > 0 ? '/admin/edit-post/' + this.state.id : '/admin/create-post';

            var myInit = {
                method: 'POST',
                mode: 'cors',
                cache: 'default',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(this.state)
            };

            fetch(uri, myInit).then(function (r) {
                return r.json();
            }).then(function (r) {
                console.log(r);
            });
            event.preventDefault();
        }
    }, {
        key: 'render',
        value: function render() {
            return React.createElement(
                'form',
                { onSubmit: this.handleSubmit },
                React.createElement(
                    'div',
                    { className: 'card-header' },
                    React.createElement(
                        'div',
                        { className: 'form-control' },
                        React.createElement('input', { type: 'text', placeholder: 'Title ...', className: 'input', name: 'title',
                            required: 'required', value: this.state.title, onChange: this.handleChange })
                    )
                ),
                React.createElement(
                    'div',
                    { className: 'card-body' },
                    React.createElement(
                        'div',
                        { className: 'form-control mb-5' },
                        React.createElement('textarea', { className: 'textarea  h-25', placeholder: 'Meta ...', name: 'metaDescription',
                            value: this.state.metaDescription, onChange: this.handleChange })
                    ),
                    React.createElement(
                        'div',
                        { className: 'form-control mb-5' },
                        React.createElement('textarea', { className: 'textarea  h-25', placeholder: 'Heading ...', name: 'chapo',
                            value: this.state.chapo, onChange: this.handleChange })
                    ),
                    React.createElement(
                        'div',
                        { className: 'form-control' },
                        React.createElement('textarea', { className: 'textarea area-full-h w-full', placeholder: 'Content ...', name: 'text',
                            value: this.state.text, onChange: this.handleChange })
                    )
                ),
                React.createElement(
                    'div',
                    { className: 'form-control mb-5' },
                    React.createElement(
                        'select',
                        { name: 'state', className: 'select select-bordered select-primary w-full max-w-xs',
                            value: this.state.state, onChange: this.handleChange },
                        React.createElement(
                            'option',
                            { value: '0' },
                            'Draft'
                        ),
                        React.createElement(
                            'option',
                            { value: '1' },
                            'Pre-Published'
                        ),
                        React.createElement(
                            'option',
                            { value: '2' },
                            'Published'
                        )
                    )
                ),
                React.createElement('input', { type: 'hidden', name: 'id', value: this.state.id }),
                React.createElement('input', { type: 'hidden', name: 'seoUrl', value: this.state.seoUrl }),
                React.createElement('input', { type: 'hidden', name: 'dateUpdated', value: this.state.dateUpdated }),
                React.createElement('input', { type: 'hidden', name: 'dateCreated', value: this.state.dateCreated }),
                React.createElement('input', { type: 'hidden', name: 'datePublished', value: this.state.datePublished }),
                React.createElement(
                    'div',
                    { className: 'form-control' },
                    React.createElement(
                        'button',
                        { type: 'submit', className: 'btn btn-primary' },
                        'SAVE'
                    )
                )
            );
        }
    }, {
        key: 'assignState',
        value: function assignState(props) {
            if (!!props.data) {
                this.state = props.data;
            } else {
                this.state = {
                    title: '',
                    chapo: '',
                    text: '',
                    state: 0,
                    id: 0,
                    seoUrl: '',
                    metaDescription: '',
                    dateUpdated: '',
                    dateCreated: '',
                    datePublished: ''

                };
            }
        }
    }]);

    return PostEditor;
}(React.Component);

var node = document.querySelector('#form-editor');
var origin = document.querySelector('#id-post').value;
if (!!origin) {
    fetch(origin).then(function (r) {
        return r.json();
    }).then(function (json) {
        ReactDOM.render(React.createElement(PostEditor, { data: json }), node);
    });
} else {
    ReactDOM.render(React.createElement(PostEditor, { data: null }), node);
}