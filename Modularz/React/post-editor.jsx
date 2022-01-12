import React from "react";
import ReactDOM from 'react-dom';
import {Editor} from '@tinymce/tinymce-react';

class PostEditor extends React.Component {


    constructor(props) {
        super(props);

        this.assignState(props);
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        const name = event.target.name;
        const value = event.target.value;
        this.setState({
            [name]: value
        });
    }

    handleSubmit(event) {
        event.preventDefault();
        const uri = this.state.id > 0 ? `/admin/edit-post/${this.state.id}` : '/admin/create-post';
        const data = this.state;
        data.state = +data.state;
        if (data.dateCreated === '')
            data.dateCreated = new Date();
        if (data.dateUpdated === '')
            data.dateUpdated = null
        
        if (data.datePublished === '')
            data.datePublished = null;
        const myInit = {
            method: 'POST',
            mode: 'cors',
            cache: 'default',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data)
        };

        fetch(uri, myInit)
            .then(r => {

                if (!r.ok) {
                    return Promise.reject(r.status);
                } else {
                    return r.json()
                }
            })
            .then(r => {
                    this.state.displaySaveBox = 'block';
                    this.setState({
                        displaySaveBox: 'block'
                    });

                    const timer = setTimeout(e => {
                        this.setState({
                            displaySaveBox: 'none'
                        });
                    }, 1600);
                },
                e => {
                    this.setState({
                        displayErrorSave: 'block'
                    });

                    const timer = setTimeout(e => {
                        this.setState({
                            displayErrorSave: 'none'
                        });
                    }, 1600);
                })

    }

    handleTmce(value, editor) {
        const name = editor.id;
        this.setState({
            [name]: value
        });
    }

    getTmce(height) {
        return {
            height: height,
            menubar: false,
            plugins: [
                'advlist autolink lists link image charmap print preview anchor',
                'searchreplace visualblocks code fullscreen',
                'insertdatetime media table paste code help wordcount'
            ],
            toolbar: 'undo redo | formatselect | ' +
                'bold italic backcolor | alignleft aligncenter ' +
                'alignright alignjustify | bullist numlist outdent indent | ' +
                'removeformat | help',
            content_style: 'body { font-family:Helvetica,Arial,sans-serif; font-size:14px }'
        };
    }

    render() {
        return (

            <form onSubmit={this.handleSubmit} className="bg-base-200 p-5">
                <div>
                    <div className="form-control">
                        <input type="text" placeholder="Title ..." className="input" name="title"
                               required="required" value={this.state.title} onChange={this.handleChange}/>
                    </div>
                </div>
                <div className="grid grid-cols-4 gap-5 mt-5">
                    <div id="left-part" className="col-span-3">


                        <div className="form-control mb-5">
                            <Editor
                                id="chapo"
                                value={this.state.chapo}
                                onEditorChange={(newValue, editor) => this.handleTmce(newValue, editor)}
                                tinymceScriptSrc="/js/tinymce/js/tinymce/tinymce.js"
                                init={this.getTmce(250)}
                            />
                        </div>
                        <div className="form-control">
                            <Editor
                                id="text"
                                value={this.state.text}
                                onEditorChange={(newValue, editor) => this.handleTmce(newValue, editor)}
                                tinymceScriptSrc="/js/tinymce/js/tinymce/tinymce.js"
                                init={this.getTmce(500)}
                            />

                        </div>

                        <input type="hidden" name="id" value={this.state.id}/>
                        <input type="hidden" name="seoUrl" value={this.state.seoUrl}/>
                        <input type="hidden" name="dateUpdated" value={this.state.dateUpdated}/>
                        <input type="hidden" name="dateCreated" value={this.state.dateCreated}/>
                        <input type="hidden" name="datePublished" value={this.state.datePublished}/>

                    </div>
                    <div id="right-part p-4">
                        <div className="form-control mb-5">
                            <textarea className="textarea h-24 textarea-bordered" value={this.state.metaDescription}
                                      onChange={this.handleChange} name="metaDescription"
                                      placeholder="meta description ..."/>
                        </div>
                        <div className="form-control mb-5">
                            <select name="state" className="select select-bordered select-primary w-full max-w-xs"
                                    value={this.state.state} onChange={this.handleChange}>

                                <option value="0">Draft</option>
                                <option value="1">Pre-Published</option>
                                <option value="2">Published</option>
                            </select>

                        </div>
                        <div className="form-control">
                            <button type="submit" className="btn btn-primary">SAVE</button>
                        </div>
                        <div style={{display: this.state.displaySaveBox}} className="alert  alert-success mt-4 ">
                            <div className="flex-1">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                                     className="w-6 h-6 mx-2 stroke-current">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                          d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-6l-2-2H5a2 2 0 00-2 2z"/>
                                </svg>
                                <span>Save OK !</span>
                            </div>
                        </div>
                        <div style={{display: this.state.displayErrorSave}} className="alert  alert-error mt-4 ">
                            <div className="flex-1">
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                                     className="w-6 h-6 mx-2 stroke-current">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                          d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-6l-2-2H5a2 2 0 00-2 2z"/>
                                </svg>
                                <span>Save not ok :(</span>
                            </div>
                        </div>
                    </div>
                </div>
            </form>


        );

    }

    assignState(props) {
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
            }
        }

        this.state.displaySaveBox = 'none';
        this.state.displayErrorSave = 'none';

    }
}


var node = document.querySelector('#form-editor');
var origin = document.querySelector('#id-post').value;
if (!!origin) {
    fetch(origin)
        .then(r => r.json())
        .then(json => {
            ReactDOM.render(<PostEditor data={json}/>, node)
        })

} else {
    ReactDOM.render(<PostEditor data={null}/>, node)
}
